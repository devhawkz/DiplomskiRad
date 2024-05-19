using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Server.Data;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Server.AuthHandler
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DataContext _context;

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            DataContext context)
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            var token = Request.Headers["Authorization"].ToString().Split(" ").Last();
            var tokenInfo = await _context.TokenInfo.FirstOrDefaultAsync(t => t.AccessToken == token);

            if (tokenInfo == null || tokenInfo.DatumIsteka < DateTime.Now)
                return AuthenticateResult.Fail("Invalid or expired token");

            var korisnickiNalog = await _context.KorisnickiNalozi.FindAsync(tokenInfo.KorisnickiNalogId);

            if (korisnickiNalog == null)
                return AuthenticateResult.Fail("Invalid token");

            var korisnickeUloge = await _context.KorisnickeUloge
                .Where(ku => ku.KorisnickiNalogId == korisnickiNalog.Id)
                .Join(_context.Uloge,
                      korisnickaUloga => korisnickaUloga.UlogaId,
                      uloga => uloga.Id,
                      (korisnickaUloga, uloga) => uloga.Naziv)
                .ToListAsync();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, korisnickiNalog.Ime!) };
            claims.AddRange(korisnickeUloge.Select(role => new Claim(ClaimTypes.Role, role!)));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
