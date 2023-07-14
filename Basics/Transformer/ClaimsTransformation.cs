using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Basics.Transformer
{
	public class ClaimsTransformation : IClaimsTransformation
	{
		public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
		{
			var roles = new List<Claim>
			{
				new Claim(ClaimTypes.Role, "Manager"),
				new Claim(ClaimTypes.Role, "SManager")
			};


			if(!principal.Identities.Any(ident=>ident.AuthenticationType == "RolesClaimIdentity")) {
				principal.AddIdentity(new ClaimsIdentity(roles, "RolesClaimIdentity"));
			}
			return Task.FromResult(principal);
		}
	}
}
