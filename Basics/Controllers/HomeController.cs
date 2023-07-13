using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }
        public IActionResult Authenticate(string returnUrl)
        {
            var grandmasClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Yahia Yagoub"),
                new Claim(ClaimTypes.Email, "Yahia@hotmail.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("Grandmas.Says", "Hello boi")
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Yahia Yagoub"),
                new Claim(ClaimTypes.MobilePhone, "+966545155868")
            };
            var grandmasClaimsIdentity = new ClaimsIdentity(grandmasClaims, "Grandmas Identity");
            var licenseClaimsIdentity = new ClaimsIdentity(licenseClaims, "Goverment Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmasClaimsIdentity, licenseClaimsIdentity });
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

            if (returnUrl != string.Empty)
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
		public async Task<IActionResult> DoStuff([FromServices] IAuthorizationService authorizationService)
		{
			// we are doing stuff here

			var customPolicy = new AuthorizationPolicyBuilder("Schema")
                .RequireClaim("Hello")
                .Build();

			var authResult = await authorizationService.AuthorizeAsync(User, customPolicy);

			if (authResult.Succeeded)
			{
				return View("Index");
			}

			return View("Index");
		}
	}
}
