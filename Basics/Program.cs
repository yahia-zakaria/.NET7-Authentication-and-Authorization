using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(config =>
{
	config.Filters.Add(new AuthorizeFilter());
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configureOptions =>
	{
		configureOptions.LoginPath = "/Home/Authenticate";
		configureOptions.Cookie.Name = "Grandmas.Cookei";
	});
builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("Claim.DOB", builder =>
    {
        builder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });
});
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();
