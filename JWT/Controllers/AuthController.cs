using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using testApi.Models;
using testApi.Services;

namespace testApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthSevice _authservice;

        public AuthController(IAuthSevice authservice)
        {
            _authservice = authservice;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 
            var result= await _authservice.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok(result);
        }
        //log in
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequsetModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authservice.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authservice.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(result);
        }
    }
}
