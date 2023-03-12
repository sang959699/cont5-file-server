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
    public class AuditController : ControllerBase
    {
        private readonly IAuditService auditService;

        public AuditController(IAuditService auditService)
        {
            this.auditService = auditService;
        }

        [HttpPost("GetAuditLog")]
        public async Task<IActionResult> GetAuditLog(GetAuditLogRequest request)
        {
            try {
                var result = await Task.Run(() => auditService.GetAuditLog(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }
    }
}
