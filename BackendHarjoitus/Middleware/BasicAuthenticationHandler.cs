using BackendHarjoitus.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BackendHarjoitus.Middleware
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserAuthenticationService userAuthenticationService) : base(options, logger, encoder, clock)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string username = "";
            string password = "";
            User? user;
            var endpoint = Context.GetEndpoint();

            var authorizeAttribute = endpoint?.Metadata.OfType<AuthorizeAttribute>();
            var allowAnonymousAttribute = endpoint?.Metadata.OfType<AllowAnonymousAttribute>();

            if (authorizeAttribute == null)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header missing");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialData = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialData).Split(new[] { ':' }, 2);
                username = credentials[0];
                password = credentials[1];

                user = await _userAuthenticationService.Authenticate(username, password);
                if (user == null)
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }
            }

            catch (Exception ex)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
