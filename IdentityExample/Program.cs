using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(config =>
{
	config.UseInMemoryDatabase("UsersDb");
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
	opt.Password.RequireUppercase = false;
	opt.Password.RequireLowercase = false;
	opt.Password.RequiredLength = 3;
	opt.Password.RequireNonAlphanumeric = false;
    opt.SignIn.RequireConfirmedEmail = true;


})
	.AddDefaultTokenProviders()
	.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(opt =>
{
	opt.LoginPath = "/Home/Login";
	opt.LogoutPath = "/Home/Logout";
});

var mailKitOptions = builder.Configuration.GetSection("Email").Get<MailKitOptions>();
builder.Services.AddMailKit(options => options.UseMailKit(mailKitOptions));

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();
