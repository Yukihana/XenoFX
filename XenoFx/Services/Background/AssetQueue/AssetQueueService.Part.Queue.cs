using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue.Models;

namespace XenoFx.Services.Background.AssetQueue;

public partial class AssetQueueService
{
    // Data

    private readonly ConcurrentQueue<AssetQueueEventContextBase> _queue = [];

    private readonly List<AssetQueueEventContextBase> _priorityRequeue = [];
    private readonly ReaderWriterLockSlim _requeueLock = new();

    private Task? _queueProcessing = null;
    private readonly SemaphoreSlim _mainLock = new(1);

    // Enqueue

    private async Task EnqueueAsync(
        AssetQueueEventContextBase context,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(context);

        // Enqueue the item
        _queue.Enqueue(context);

        // Trigger a start
        await TryStartAsync(ctoken);
    }

    // Start

    private async Task TryStartAsync(
        CancellationToken ctoken = default)
    {
        await _mainLock.WaitAsync(ctoken);
        try
        {
            // Cleanup
            if (_queueProcessing is not null)
            {
                //  If work is still going on, bail. (it will automatically pick up new jobs on upcoming iterations)
                if (!_queueProcessing.IsCompleted)
                    return;

                // If work is faulted, log the error and dereference it.
                if (_queueProcessing.IsFaulted)
                {
                    if (_queueProcessing.Exception is Exception ex)
                        _logger.LogError(ex, "Queue processing task faulted.");
                    else
                        _logger.LogError(_queueProcessing.Exception, "Queue processing task faulted.");
                }

                // Since it can be concluded that the task is done, dereference it.
                _queueProcessing = null;
            }

            // Start the actual processing for this job-call here.
            var shutdownToken = _shutdownTokenSource.Token;
            _queueProcessing = Task.Run(async () => await ProcessQueueAsync(shutdownToken), ctoken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start queue processing.");
            return;
        }
        finally
        {
            _mainLock.Release();
        }
    }

    // Processing Loop

    private async Task ProcessQueueAsync(
        CancellationToken ctoken = default)
    {
        _logger.LogInformation("Pocessing starting...");
        List<Task> tasks = [];
        int maxTasks = System.Environment.ProcessorCount;

        while (!ctoken.IsCancellationRequested)
        {
            // Lazy cleanup
            CleanupCompletedTasks(tasks);

            // If we have too many tasks, wait for one to complete.
            if (tasks.Count > maxTasks)
                await Task.WhenAny(tasks);

            // Check if we can dequeue a new item.
            // prority dequeue first (for items waiting). else:
            if (_queue.TryDequeue(out var item))
            {
                var task = ProcessItemAsync(item, ctoken);
                tasks.Add(task);
                continue;
            }

            // Wait for active task count and queue to receed to zero.
            if (tasks.Count == 0 && _queue.IsEmpty)
            {
                // If resync isn't required, shutdown this subroutine. (the next enqueue will restart it)
                if (!_requireResync)
                    break;

                // Trigger a resync
                await TryResyncAsync(ctoken);
            }
            else
            {
                // Wait for a short time before checking again.
                await Task.Delay(100, ctoken);
            }
        }

        _logger.LogInformation("Processing stopped.");
    }

    // Dequeue

    private bool TryDequeue([NotNullWhen(true)] out AssetQueueEventContextBase? context)
    {
        // Check priority requeues first
        if (TryPriorityDequeue(out var priorityItem))
        {
            context = priorityItem;
            return true;
        }

        // Try to dequeue an item
        return _queue.TryDequeue(out context);
    }

    // Cleanup

    private void CleanupCompletedTasks(List<Task> tasks)
    {
        // Clear the queue
        List<Task> removeList = [.. tasks.Where(t => t.IsCompleted)];
        tasks.RemoveAll(removeList.Contains);

        // Summarize cleanup results
        int total = removeList.Count;
        int faulted = removeList.Count(t => t.IsFaulted);
        _logger.LogDebug("Cleanedup total {total} tasks, including {faulted} faulted tasks.", total, faulted);
    }

    // Priority Queue

    private void PriorityEnqueue(AssetQueueEventContextBase context)
    {
        _requeueLock.EnterWriteLock();
        try
        {
            _priorityRequeue.Add(context);
        }
        finally { _requeueLock.ExitWriteLock(); }
    }

    private bool TryPriorityDequeue([NotNullWhen(true)] out AssetQueueEventContextBase? context)
    {
        context = null;

        _requeueLock.EnterUpgradeableReadLock();
        try
        {
            context = _priorityRequeue.FirstOrDefault(x => x.ReevaluateAfter < DateTime.UtcNow);
            if (context != null)
            {
                _requeueLock.EnterWriteLock();
                try
                {
                    _priorityRequeue.Remove(context);
                }
                finally { _requeueLock.ExitWriteLock(); }

                return true;
            }
        }
        finally { _requeueLock.ExitUpgradeableReadLock(); }
        return false;
    }
}