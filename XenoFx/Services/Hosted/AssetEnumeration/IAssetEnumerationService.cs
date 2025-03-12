using Microsoft.Extensions.Hosting;
using System;

namespace XenoFx.Services.Hosted.AssetEnumeration;

public interface IAssetEnumerationService : IHostedService, IDisposable
{
    string[] EnumerateFiles();
}