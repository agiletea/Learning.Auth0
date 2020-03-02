using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgileTea.Auth0.Web.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[]
            {
                "Test1",
                "Test2"
            };
        }

        [HttpPost]
        public void Post([FromBody] string selectedAsset)
        {
            var claims = GetClaims(selectedAsset);
        }

        private static Claim[] GetClaims(string asset)
        {
            return asset == "Test1" ? new[]
            {
                new Claim("CanX", "true", ClaimValueTypes.Boolean),
                new Claim("CanY", "true", ClaimValueTypes.Boolean)
            } : new[]
            {
                new Claim("CanW","true", ClaimValueTypes.Boolean),
                new Claim("CanZ", "true", ClaimValueTypes.Boolean)
            };
        }
    }
}
