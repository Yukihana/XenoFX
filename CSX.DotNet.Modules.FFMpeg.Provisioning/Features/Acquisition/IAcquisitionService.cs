using CSX.DotNet.Common.Abstractions;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Acquisition;

public interface IAcquisitionService
    : IFFMpegProvider
{
    // Automatically inherits the contract of the cross-module abstraction
}