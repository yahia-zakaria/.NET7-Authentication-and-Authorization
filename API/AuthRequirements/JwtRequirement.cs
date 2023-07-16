using Microsoft.AspNetCore.Authorization;

namespace API.AuthRequirements
{
	public class JwtRequirement : IAuthorizationRequirement
	{
	}
	public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
	{
		private readonly HttpClient _httpClient;
		private readonly HttpContext _httpContext;
        public JwtRequirementHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
				_httpClient = httpClientFactory.CreateClient();
			_httpContext = httpContextAccessor.HttpContext;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtRequirement requirement)
		{
			if(_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
			{
				var accessToken = authHeader.ToString().Split(' ')[1];
				if(accessToken is not null) {
					var response = await _httpClient.GetAsync($"http://localhost:5093/oauth/validate?access_token={accessToken}");
					if(response.StatusCode == System.Net.HttpStatusCode.OK)
					{
						context.Succeed(requirement);
					}
				}
			}
		}
	}
}
