

using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews()
	.AddRazorRuntimeCompilation();

builder.Services.AddAuthentication(config =>
{
	//to chech the cookie if the user is authenticated
	config.DefaultAuthenticateScheme = "ClientAuthCookie";
	//when we sign in it will deal out cookie
	config.DefaultSignInScheme = "ClientAuthCookie";
	//to chech if the user is allowed to do something 
	config.DefaultChallengeScheme = "OAuthServer";
})
	.AddCookie("ClientAuthCookie")
	.AddOAuth("OAuthServer", config =>
	{
		config.CallbackPath = "/oauth/authorize";
		config.ClientId = "client_id";
		config.ClientSecret = "client_secret";
		config.CallbackPath = "/oauth/callback";
		config.AuthorizationEndpoint = "http://localhost:5093/oauth/authorize";
		config.TokenEndpoint = "http://localhost:5093/oauth/token";
		config.SaveTokens = true;
		config.CorrelationCookie.SameSite = SameSiteMode.Lax;
		config.Events = new OAuthEvents()
		{
			OnCreatingTicket = context =>
			{
				var accessToken = context.AccessToken;
				var base64payload = accessToken?.Split('.')[1];
				var bytes = Convert.FromBase64String(base64payload);
				var jsonPayload = Encoding.UTF8.GetString(bytes);
				var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonPayload);
				if(claims is not  null)
				foreach (var claim in claims)
				{
					context?.Identity?.AddClaim(new Claim(claim.Key, claim.Value?.ToString()));
				}

				return Task.CompletedTask;
			}
		};
	});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();
