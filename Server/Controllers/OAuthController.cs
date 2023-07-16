using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
	public class OAuthController : Controller
	{
		private readonly IConfiguration _configuration;

		public OAuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		[HttpGet]
		public IActionResult Authorize(
		  string response_type, // authorization flow type 
			string client_id, // client id
			string redirect_uri,
			string scope, // what info I want = email,grandma,tel
			string state)
		{
			var query = new QueryBuilder();
			query.Add("redirectUri", redirect_uri);
			
			query.Add("state", state);
			return View(model: query.ToString());
		}
		[HttpPost]
		public IActionResult Authorize(
			string username,
			string redirectUri,
			string state
			)
		{
			const string code = "BABAABABABA";

			var query = new QueryBuilder();
			query.Add("code", code);
			query.Add("state", state);


			return Redirect($"{redirectUri}{query.ToString()}");
		}

		[AllowAnonymous]
		public IActionResult Token(
			string grant_type, // flow of access_token request
			string code, // confirmation of the authentication process
			string redirect_uri,
			string client_id,
			string refresh_token)
		{
			// some mechanism for validating the code

			var claims = new[]
		  {
				new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
				new Claim("granny", "cookie")
			};
			var issuer = _configuration.GetSection("JwtToken")["Issuer"];
			var audiance = _configuration.GetSection("JwtToken")["Audience"];
			var secret = _configuration.GetSection("JwtToken")["Secret"];
			var secretBytes = Encoding.UTF8.GetBytes(secret);
			var key = new SymmetricSecurityKey(secretBytes);
			var algorithm = SecurityAlgorithms.HmacSha256;

			var signingCredentials = new SigningCredentials(key, algorithm);

			var token = new JwtSecurityToken(
				issuer,
				audiance,
				claims,
				notBefore: DateTime.Now,
				expires: DateTime.Now.AddMinutes(5),
				signingCredentials);

			var access_token = new JwtSecurityTokenHandler().WriteToken(token);

			var responseObject = new
			{
				access_token,
				token_type = "Bearer",
				raw_claim = "oauthTutorial"
			};

			var responseJson = JsonConvert.SerializeObject(responseObject);

			return Ok(responseObject);
		}

		public IActionResult Validate()
		{
			if(HttpContext.Request.Query.TryGetValue("access_token", out var accessToken))
			{
				return Ok();
			}
			return BadRequest();
		}
	}

}
