using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cont5.Models.Login;
using Microsoft.AspNetCore.Authorization;
using Cont5.Services;
using System;

namespace Cont5.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest request)
        {
            try {
                var result = await Task.Run(() => loginService.Authenticate(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("ValidateToken")]
        public async Task<IActionResult> ValidateToken(ValidateTokenRequest request) {
            try {
                var result = await Task.Run(() => loginService.ValidateToken(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken()
        {
            try {
                var result = await Task.Run(() => loginService.RenewToken());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetRoleId")]
        public async Task<IActionResult> GetRoleId()
        {
            try {
                var result = await Task.Run(() => loginService.GetRoleId());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        // [AllowAnonymous]
        // [HttpPost("HashPassword")]
        // public async Task<IActionResult> HashPassword(HashPasswordRequest request)
        // {
        //     var result = await Task.Run(()=> loginService.HashPassword(request));
        //     return Ok(result);
        // }
    }
}
