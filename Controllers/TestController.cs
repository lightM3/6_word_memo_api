using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WordMemoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        
        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtected()
        {
            var username = User.Identity?.Name;
            return Ok($"Merhaba {username}, korumalı alandasın!");
        }

        [AllowAnonymous]
        [HttpGet("public")]
        public IActionResult GetPublic()
        {
            return Ok("Burası herkese açık bir endpoint.");
        }
    }
}
