using System;

namespace XenoFx.Services.Api.StateMonitor;

public interface IStateMonitorService
{
    ulong GetAssetRepositoryStateIndex();

    DateTime GetAssetRepositoryLastModified();
}