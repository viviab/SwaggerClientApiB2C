using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SwaggerClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var claims = HttpContext.User.Claims;
            var username = claims.FirstOrDefault(c=> c.Type == "name")?.Value;
            var tenant = claims.FirstOrDefault(c => c.Type == "emails")?.Value;

            return Ok(new[] {username, tenant});}
    }
}
