using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SHSDP.API.Services;

public class LoginTokenAuthenticationHandlerSvc : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public LoginTokenAuthenticationHandlerSvc(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder) { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        String clientIpAddress = Context.Connection.RemoteIpAddress!.ToString();

        if (Request.Headers.ContainsKey("Authorization") == false)
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        String authHeader = Request.Headers["Authorization"].ToString();
        if (authHeader.StartsWith("Bearer ") == false)
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        String token = authHeader.Substring("Bearer ".Length).Trim();

        if (String.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail("Token is missing");
        }
        
        if(token.Equals(Shared.Constants.AUTH_TOKEN) == false)
        {
            return AuthenticateResult.Fail("Invalid Token");
        }
        
        ClaimsIdentity identity = new ClaimsIdentity([], Scheme.Name);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }
}