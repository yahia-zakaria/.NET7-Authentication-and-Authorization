using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews()
	.AddRazorRuntimeCompilation();

builder.Services.AddAuthentication("OAuth")
	.AddJwtBearer("OAuth", config =>
	{
		config.Events = new JwtBearerEvents()
		{
			OnMessageReceived = context =>
			{
				if (context.Request.Query.ContainsKey("access_token"))
				{
					context.Token = context.Request.Query["access_token"];
				}
				return Task.CompletedTask;
			}
		};
		var keyBytes = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtToken")["Secret"]);
		var key = new SymmetricSecurityKey(keyBytes);
		config.TokenValidationParameters = new TokenValidationParameters
		{
			ClockSkew = TimeSpan.Zero,
			ValidAudience = builder.Configuration.GetSection("JwtToken")["Audience"],
			ValidIssuer = builder.Configuration.GetSection("JwtToken")["Issuer"],
			IssuerSigningKey = key
		};
	});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();
