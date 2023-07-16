using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
	public class HomeController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly HttpClient _httpClient;

		public HomeController(IConfiguration configuration, IHttpClientFactory httpClient)
		{
			_configuration = configuration;
			_httpClient = httpClient.CreateClient();
		}

		public IActionResult Index()
		{
			return View();
		}
		[Authorize]
		public async Task<IActionResult> Secret()
		{
			var token = await HttpContext.GetTokenAsync("access_token");
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
			var response1 = await _httpClient.GetAsync("http://localhost:5093/Secret/Index");
			var response2 = await _httpClient.GetAsync("http://localhost:5266/Secret/Index");
			return View();
		}

	}
}
