using Identity.Admin.Api.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Admin.Api.Services
{
    public class ProfileService : IProfileService
    {
        public readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var user = await _userManager.FindByIdAsync(subjectId) ?? throw new ArgumentNullException("Invalid subject identifier");
            var claims = GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var user = await _userManager.FindByIdAsync(subjectId) ?? throw new ArgumentNullException("Invalid subject identifier");
            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var securityStamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).FirstOrDefault();
                    if(securityStamp != null)
                    {
                        var dbSecurityStamp = await _userManager.GetSecurityStampAsync(user);
                        if(dbSecurityStamp != securityStamp)
                            return;
                    }
                }
            }
        }

        private IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user) {
            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtClaimTypes.Name, user.UserName),
            };

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));

            if (!string.IsNullOrWhiteSpace(user.LastName))
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    //new Claim(JwtClaimTypes.Email, user.EmailConfirmed ? "true" : "false", claims[])

                });
            }

            return claims;

        }
    }
}
