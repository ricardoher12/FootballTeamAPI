using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace FootballTeamsAPI.Authentication
{
    public class BasicAuthenticationHandler
     : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(value!);
                if (!authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult(AuthenticateResult.Fail("Invalid auth scheme"));

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                var username = credentials[0];
                var password = credentials[1];

                var expectedUser = _configuration["BasicAuth:Username"];
                var expectedPass = _configuration["BasicAuth:Password"];

                if (username != expectedUser || password != expectedPass)
                    return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));

                var claims = new[]
                {
                new Claim(ClaimTypes.Name, username)
            };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}
