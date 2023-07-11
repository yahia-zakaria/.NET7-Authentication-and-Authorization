using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}
		[Authorize]
		public IActionResult Index()
		{
			return View();
		}
		[Authorize]
		public IActionResult Secret()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{
			var user = await _userManager.FindByNameAsync(username);

			if(user is not null)
			{
				var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
				if (signInResult.Succeeded)
				{
					return RedirectToAction("Secret");
				}
			}
			ViewBag.username = username;
			ViewBag.password = password;
			return View();
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(string username, string password)
		{
			var user = new IdentityUser { UserName = username, Email = username };
			var result = await _userManager.CreateAsync(user, password);
			if (result.Succeeded) {
				var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
				if(signInResult.Succeeded)
				{	
					return RedirectToAction("Secret");
				}
			}
			ViewBag.username = username;
			ViewBag.password = password;
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index");

		}
	}
}
