using LifeTag.Contracts.DTOs;
using LifeTag.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LifeTag.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
        {
            var result = await _authService.SignUpAsync(signUpDto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
        {
            var result = await _authService.SignInAsync(signInDto);
            if (!result.Success) return Unauthorized(result);
            return Ok(result);
        }
    }
}
