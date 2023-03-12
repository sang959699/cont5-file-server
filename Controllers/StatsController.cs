using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cont5.Services;
using Cont5.Models.Audit;
using System;

namespace Cont5.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService statsService;

        public StatsController(IStatsService statsService)
        {
            this.statsService = statsService;
        }

        [HttpPost("GetWatchedStats")]
        public async Task<IActionResult> GetWatchedStats()
        {
            try {
                var result = await Task.Run(() => statsService.GetWatchedStats());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetPendingStats")]
        public async Task<IActionResult> GetPendingStats()
        {
            try {
                var result = await Task.Run(() => statsService.GetPendingStats());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }
    }
}
