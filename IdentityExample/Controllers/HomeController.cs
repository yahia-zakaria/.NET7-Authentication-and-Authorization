using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _mailService;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IEmailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
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

            if (user is not null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
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
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());
                await _mailService.SendAsync(user.Email, "Verify Your Email Please", $"<a href=\"{link}\">Verify Email</a>", true);
                return RedirectToAction(nameof(EmailVerification));
            }
            ViewBag.username = username;
            ViewBag.password = password;
            return View();
        }

        public IActionResult EmailVerification() => View();
        public async Task<IActionResult> VerifyEmail(string userId, string code) {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                return BadRequest();
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded) { 
            return RedirectToAction(nameof(Login));    
            }
            return BadRequest();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");

        }
    }
}
