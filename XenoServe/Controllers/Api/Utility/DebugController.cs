using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Database.CacheDb;

namespace XenoServe.Controllers.Api.Utility;

[Route("api/[controller]")]
[ApiController]
public class DebugController : ControllerBase
{
    private readonly CacheDbContext _dbContext;

    public DebugController(CacheDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> ShowDataAsync(CancellationToken ctoken = default)
        => Ok(await _dbContext.AssetPresences.ToListAsync(ctoken));
}