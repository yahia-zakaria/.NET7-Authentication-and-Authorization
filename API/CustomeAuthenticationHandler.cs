using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace API
{
	public class CustomeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public CustomeAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
			ILoggerFactory logger, 
			UrlEncoder encoder, 
			ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			return Task.FromResult(AuthenticateResult.Fail("Failed Authentication"));
		}
	}
}
