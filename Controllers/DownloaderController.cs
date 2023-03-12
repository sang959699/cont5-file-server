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
    public class DownloaderController : ControllerBase
    {
        private readonly IDownloaderService downloaderService;

        public DownloaderController(IDownloaderService downloaderService)
        {
            this.downloaderService = downloaderService;
        }

        [HttpPost("GetAriaNgUrl")]
        public async Task<IActionResult> GetAriaNgUrl()
        {
            try {
                var result = await Task.Run(() => downloaderService.GetAriaNgUrl());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetFailedDownloadCount")]
        public async Task<IActionResult> GetFailedDownloadCount()
        {
            try {
                var result = await Task.Run(() => downloaderService.GetFailedDownloadCount());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }
    }
}
