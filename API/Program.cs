using API;
using API.AuthRequirements;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddAuthentication("DefaultAuth")
				.AddScheme<AuthenticationSchemeOptions, CustomeAuthenticationHandler>("DefaultAuth", null);
builder.Services.AddAuthorization(config =>
{
	config.DefaultPolicy = new AuthorizationPolicyBuilder()
	.AddRequirements(new JwtRequirement())
	.Build();
});
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, JwtRequirementHandler>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
