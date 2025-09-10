using CSX.DotNet.Common.EFC.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSX.DotNet.Modules.FileIndexing.Shared.Database;

public class IndicesDbContext : BaseDbContext<IndicesDbContext>
{
    public IndicesDbContext(
        DbContextOptions options,
        ILogger<IndicesDbContext> logger)
        : base(options, logger)
    {
    }
}