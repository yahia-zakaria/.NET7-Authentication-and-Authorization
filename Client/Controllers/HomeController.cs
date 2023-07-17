using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Controllers
{
	public class HomeController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _httpClientFactory;

		public HomeController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
		{
			_configuration = configuration;
			_httpClientFactory = httpClientFactory;
		}

		public IActionResult Index()
		{
			return View();
		}
		[Authorize]
		public async Task<IActionResult> Secret()
		{
			var serverResponse = await AccessTokenRefreshWrapper(
				() => SecuredGetRequest("http://localhost:5093/secret/index"));

			var apiResponse = await AccessTokenRefreshWrapper(
				() => SecuredGetRequest("http://localhost:5266/secret/index"));

			return View();
		}

		public async Task<HttpResponseMessage> SecuredGetRequest(string url)
		{
			var client = _httpClientFactory.CreateClient();
			var token = await HttpContext.GetTokenAsync("access_token");
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
			var responseMessage = await client.GetAsync(url);
			return responseMessage;
		}

		public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
	Func<Task<HttpResponseMessage>> initialRequest)
		{
			var response = await initialRequest();

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				await RefreshAccessToken();
				response = await initialRequest();
			}

			return response;
		}

		public async Task RefreshAccessToken()
		{
			var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
			var client = _httpClientFactory.CreateClient();
			var requestData = new Dictionary<string, string>()
			{
				["grant_type"] = "refresh_token",
				["refresh_token"] = refreshToken ?? string.Empty
			};

			var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5093/oauth/token")
			{
				Content = new FormUrlEncodedContent(requestData)
			};

			var basicCredentials = "username:password";
			var encodedCredentials = Encoding.UTF8.GetBytes(basicCredentials);
			var base64Credentials = Convert.ToBase64String(encodedCredentials);

			req.Headers.Add("Authorization", $"Basic {base64Credentials}");

			var response = await client.SendAsync(req);

			var responseString = await response.Content.ReadAsStringAsync();
			var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);

			var newAccessToken = responseData.GetValueOrDefault("access_token");
			var newRefreshToken = responseData.GetValueOrDefault("refresh_token");

			var authInfo = await HttpContext.AuthenticateAsync("ClientAuthCookie");

			authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
			authInfo.Properties.UpdateTokenValue("refresh_token", newRefreshToken);

			await HttpContext.SignInAsync("ClientAuthCookie", authInfo.Principal, authInfo.Properties);
		}


	}
}
