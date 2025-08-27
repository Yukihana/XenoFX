using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.FFMpeg.Provisioning.Features.Configuration;

public interface IConfigurationService
{
    string BinariesDirectory { get; }
    string SharedCacheDirectory { get; }
}