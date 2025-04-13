using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using XenoFx.Services.Background.AssetQueue.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XenoFx.Services.Background.AssetQueue;

public partial class AssetQueueService
{
    // Data

    private readonly ConcurrentQueue<AssetQueueEventContextBase> _queue = [];
    private Task? _queueProcessing = null;
    private readonly SemaphoreSlim _lock = new(1);

    // Enqueue / Start

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

    private async Task TryStartAsync(
        CancellationToken ctoken = default)
    {
        await _lock.WaitAsync(ctoken);
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
            _lock.Release();
        }
    }

    // Processing Loop

    private async Task ProcessQueueAsync(
        CancellationToken ctoken = default)
    {
        _logger.LogInformation("Pocessing starting...");
        List<Task<AssetQueueEventContextBase>> tasks = [];
        List<Task<AssetQueueEventContextBase>> removeList = [];
        int maxTasks = System.Environment.ProcessorCount;

        while (!ctoken.IsCancellationRequested)
        {
            // Remove completed tasks.
            await CleanupCompletedTasksAsync(tasks, ctoken);

            // If we have too many tasks, wait for one to complete.
            if (tasks.Count > maxTasks)
                await Task.WhenAny(tasks);

            // Check if we can dequeue a new item.
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

    private async Task CleanupCompletedTasksAsync(List<Task<AssetQueueEventContextBase>> tasks, CancellationToken ctoken)
    {
        // ProcessItemAsync will handle the exceptions. A scaffold is unnecessary.
        List<Task<AssetQueueEventContextBase>> removeList = [];
        foreach (var task in tasks.Where(t => t.IsCompleted))
        {
            try
            {
                var result = await task;
                if (result.ReevaluationRequired)
                {
                    unchecked { result.ReevaluationCount++; }
                    _logger.LogWarning("Queuing task for re-evaluation # {count}: {context}", result.ReevaluationCount, result);
                    await EnqueueAsync(result, ctoken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing scaffold failed to contain the error and resulted in the loss of input data for the task: {task}", task);
            }
            finally
            {
                removeList.Add(task);
            }
        }
        tasks.RemoveAll(removeList.Contains);
    }

    // Process Item

    private async Task<AssetQueueEventContextBase> ProcessItemAsync(
        AssetQueueEventContextBase context,
        CancellationToken ctoken = default)
    {
        try
        {
            context.ReevaluationRequired = await RouteProcessingAsync(context, ctoken);
            return context;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Asset indexing was canceled for: {context}", context);
            context.ReevaluationRequired = false;
            return context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Asset indexing faulted for: {context}", context);
            context.ReevaluationRequired = true;
            return context;
        }
    }

    private async Task<bool> RouteProcessingAsync(
        AssetQueueEventContextBase context,
        CancellationToken ctoken = default)
    {
        // Resync

        if (context is AssetResyncEventContext resyncContext)
        {
            return await _assetIndexing.IndexResyncEventAsync(
                relativePath: resyncContext.RelativePath,
                ctoken: ctoken);
        }

        // Watcher

        if (context is AssetCreatedEventContext createdContext)
        {
            return await _assetIndexing.IndexCreateEventAsync(
                relativePath: createdContext.RelativePath,
                args: createdContext.EventArgs,
                ctoken: ctoken);
        }

        if (context is AssetDeletedEventContext deletedContext)
        {
            return await _assetIndexing.IndexDeleteEventAsync(
                relativePath: deletedContext.RelativePath,
                args: deletedContext.EventArgs,
                ctoken: ctoken);
        }

        if (context is AssetModifiedEventContext modifiedContext)
        {
            return await _assetIndexing.IndexModifyEventAsync(
                relativePath: modifiedContext.RelativePath,
                args: modifiedContext.EventArgs,
                ctoken: ctoken);
        }

        if (context is AssetRenamedEventContext renamedContext)
        {
            return await _assetIndexing.IndexRenameEventAsync(
                renamedContext.OldRelativePath,
                renamedContext.NewRelativePath,
                renamedContext.EventArgs,
                ctoken);
        }

        // Upload

        if (context is AssetUploadedEventContext uploadContext)
        {
            if (context.ReevaluationCount > 0)
            {
                _logger.LogWarning("Upload event was already processed: {context}", context);
                return false;
            }
            return await _assetIndexing.IndexUploadEventAsync(
                uploadContext.RelativePath,
                uploadContext.ReportedFilename,
                uploadContext.Title,
                uploadContext.MimeType,
                uploadContext.PageUrl,
                uploadContext.DataUrl,
                ctoken);
        }

        throw new NotImplementedException($"Processing for {context.GetType()} is not implemented.");
    }
}