using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
	public class HomeController : Controller
	{
		private readonly IConfiguration _configuration;

		public HomeController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IActionResult Index()
		{
			return View();
		}
		[Authorize]
		public IActionResult Secret()
		{
			return View();
		}

		[AllowAnonymous]
		public IActionResult Authenticate(string returnUrl)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.Name, "Yahia Yagoub"),
				new Claim(ClaimTypes.Email, "Yahia@hotmail.com"),
				new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
			};

			var keyBytes = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtToken")["Secret"]);
			var key = new SymmetricSecurityKey(keyBytes);

			var jwtSecurity = new JwtSecurityToken(
				_configuration.GetSection("JwtToken")["Issuer"],
				_configuration.GetSection("JwtToken")["Audience"],
				claims,
				notBefore: DateTime.Now,
				expires: DateTime.Now.AddHours(1),
				new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
				);

			var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurity);
			return Ok(token);
		}
	}
}
