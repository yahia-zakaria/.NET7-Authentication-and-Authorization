using IdentityExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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


})
	.AddDefaultTokenProviders()
	.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(opt =>
{
	opt.LoginPath = "/Home/Login";
	opt.LogoutPath = "/Home/Logout";
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();
