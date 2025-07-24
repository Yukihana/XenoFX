using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Background.AssetQueue.Models;

namespace XenoFx.Services.Background.AssetQueue;

public partial class AssetQueueService
{
    // Process Item

    private async Task ProcessItemAsync(
        AssetQueueEventContextBase context,
        CancellationToken ctoken = default)
    {
        // Set re-evaluation requirement to false initially
        context.ReevaluationRequired = false;

        try
        {
            // Run the task
            context.ReevaluationRequired = await RouteProcessingAsync(context, ctoken);

            // Task completed successfully, exit early
            if (!context.ReevaluationRequired)
                return;
        }
        catch (OperationCanceledException ex)
        {
            // Bail early on cancelled
            _logger.LogError(ex, "Indexing was canceled for: {context}", context);
            return;
        }
        catch (Exception ex)
        {
            // Handle unexpected exceptions
            _logger.LogError(ex, "Indexing faulted for: {context}", context);
            context.ReevaluationRequired = true;
        }

        // Handle re-evaluation logic
        unchecked { context.ReevaluationCount++; }
        double secondsDelay = Math.Min(context.ReevaluationCount * 2, 300);

        // Set re-evaluation time
        context.ReevaluateAfter = DateTime.UtcNow.AddSeconds(secondsDelay);

        // Queue for reevaluation and log it.
        PriorityEnqueue(context);
        _logger.LogInformation("Queuing indexing task for re-evaluation after {sec} seconds: {context}", secondsDelay, context);
    }

    private async Task<bool> RouteProcessingAsync(
        AssetQueueEventContextBase context,
        CancellationToken ctoken = default)
    {
        // Resync

        if (context is AssetResyncEventContext resyncContext)
        {
            int count = await _assetIndexing.IndexResyncEventAsync(
                fileList: resyncContext.FileList,
                ctoken: ctoken);
            return count > 0;
        }

        // Watcher

        if (context is AssetCreatedEventContext createdContext)
        {
            return await _assetIndexing.IndexCreateEventAsync(
                fullPath: createdContext.FullPath,
                ctoken: ctoken);
        }

        if (context is AssetDeletedEventContext deletedContext)
        {
            return await _assetIndexing.IndexDeleteEventAsync(
                fullPath: deletedContext.FullPath,
                ctoken: ctoken);
        }

        if (context is AssetModifiedEventContext modifiedContext)
        {
            return await _assetIndexing.IndexModifyEventAsync(
                fullPath: modifiedContext.FullPath,
                ctoken: ctoken);
        }

        if (context is AssetRenamedEventContext renamedContext)
        {
            return await _assetIndexing.IndexRenameEventAsync(
                fullPath: renamedContext.FullPath,
                oldFullPath: renamedContext.OldFullPath,
                ctoken);
        }

        // Upload

        if (context is AssetUploadedEventContext uploadContext)
        {
            return await _assetIndexing.IndexUploadEventAsync(
                uploadMetadataPath: uploadContext.UploadMetadataPath,
                cleanupCallback: uploadContext.CleanupCallback,
                ctoken: ctoken);
        }

        throw new NotImplementedException($"Processing for {context.GetType()} is not implemented.");
    }
}