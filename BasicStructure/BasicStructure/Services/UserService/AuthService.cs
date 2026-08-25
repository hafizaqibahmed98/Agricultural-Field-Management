using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Services.Services;
using Services.Models;
using System.Net;

namespace BasicStructure.Services.UserService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole<int>> _role;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        public DataContext _context { get; }

        public AuthService(UserManager<ApplicationUser> user,
            RoleManager<IdentityRole<int>> role,
            IEmailService emailService,
            IConfiguration config,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            DataContext context
            )
        {
            _user = user;
            _role = role;
            _emailService = emailService;
            _config = config;
            _context = context;
        }
        public async Task<bool> RegisterUser(RegisterUserDTO user, int roleId, HttpContext httpContext)
        {
            var role = await _role.FindByIdAsync(roleId.ToString());
            if (role != null)
            {
                var identityUser = new ApplicationUser
                {
                    Email = user.Email,
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };
                var result = await _user.CreateAsync(identityUser, user.Password);
                if (result.Succeeded)
                {
                    result = await _user.AddToRoleAsync(identityUser, role.Name);
                    //Add Token to Verify the email....
                    var token = await _user.GenerateEmailConfirmationTokenAsync(identityUser);
                    //var confirmationLink = Url.Action(nameof(ConfirmEmail), "Register", new { token, email = user.Email }, Request.Scheme);
                    //var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext();
                    //var confirmationLink = _linkGenerator.GetPathByAction(httpContext ,"ConfirmEmail", "Register", new { token, email = user.Email });
                    var scheme = httpContext.Request.Scheme;
                    var host = httpContext.Request.Host;
                    var pathBase = httpContext.Request.PathBase;

                    string encodedToken = WebUtility.UrlEncode(token);
                    string encodedEmail = WebUtility.UrlEncode(identityUser.Email);

                    var confirmationLink = $"{scheme}://{host}{pathBase}/api/AuthUser/ConfirmEmail?token={encodedToken}&email={encodedEmail}";
                    var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                    _emailService.SendEmail(message);
                }
                return result.Succeeded;
            }
            return false;
        }

        public async Task<bool> ConfirmEmail(string token, string email)
        {
            var user = await _user.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _user.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ForgotPassword( string email, HttpContext httpContext)
        {
            var user = await _user.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _user.GeneratePasswordResetTokenAsync(user);
                var scheme = httpContext.Request.Scheme;
                var host = httpContext.Request.Host;
                var pathBase = httpContext.Request.PathBase;

                //string encodedToken = WebUtility.UrlEncode(token);
                string encodedToken = token;
                string encodedEmail = WebUtility.UrlEncode(email);

                var confirmationLink = $"{scheme}://{host}{pathBase}/api/AuthUser/ResetPassword?token={encodedToken}&email={encodedEmail}";
                var message = new Message(new string[] { user.Email! }, "Reset Password link", confirmationLink!);
                _emailService.SendEmail(message);
                return true;
            }
            return false;
        }

        public async Task<bool> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _user.FindByEmailAsync(resetPassword.email);
            if (user != null)
            { 
                var resetPasswordResult = await _user.ResetPasswordAsync(user, resetPassword.token, resetPassword.ConfirmPassword);
                if (resetPasswordResult?.Succeeded == true)
                    return true;
            }
            return false;
        }


        public async Task<bool> LoginUser(Models.LoginUser user)
        {
            var identityUser = await _user.FindByEmailAsync(user.Email);
            if (identityUser == null)
                return false;
            return await _user.CheckPasswordAsync(identityUser, user.Password);
        }

        public string GenerateTokenString(Models.LoginUser user)
        {
            var jwtKey = GetRequiredConfiguration("Jwt:Key");
            var jwtIssuer = GetRequiredConfiguration("Jwt:Issuer");
            var jwtAudience = GetRequiredConfiguration("Jwt:Audience");
            var identityUser = _user.FindByEmailAsync(user.Email).Result;
            var roleId = "dimmy";
            if (identityUser != null)
            {
                // Get user roles
                var roles = _user.GetRolesAsync(identityUser).Result;
                foreach (var roleName in roles)
                {
                    var role = _role.FindByNameAsync(roleName);

                    if (role != null)
                    {
                        // Access the role ID using role.Id
                        roleId = role.Id.ToString();
                        // Do something with roleId
                    }
                }
                // Add roles as claims
            }
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", identityUser.Id.ToString()),
                new Claim("roleId", roleId),
            };
            IEnumerable<System.Security.Claims.Claim> claims = userClaims;
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes
                (jwtKey)
                );
            SigningCredentials SigningCred = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256
                );
            var securityoken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: jwtIssuer,
                audience: jwtAudience,
                signingCredentials: SigningCred
                );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityoken);
            return tokenString;
        }

        private string GetRequiredConfiguration(string key)
        {
            var value = _config[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Required authentication configuration '{key}' is missing. Set it with an environment variable or .NET User Secrets.");
            }

            return value;
        }

        public async Task<bool> CheckPermission(User user, int apiId)
        {
            var userId = user?.Id;
            var roleId = user?.RoleId;
            if(userId != null && roleId != null)
            {
                var dbPermissions = await _context.Permissions.ToListAsync();
                foreach (var permission in dbPermissions)
                {
                    if(permission.APIId == apiId && permission.IdentityRoleId == roleId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public User DecodeToken(string token)
        {
            // Decode the token
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var roleClaims = jsonToken?.Claims.Where(claim => claim.Type == "roleId").Select(claim => claim.Value).ToList();
            var idClaims = jsonToken?.Claims.Where(claim => claim.Type == "UserId").Select(claim => claim.Value).ToList();
            User dataObject = new User()
            {
                Id = int.Parse(idClaims[0]),
                RoleId = int.Parse(roleClaims[0]),
            };
            return dataObject;
        }
    }
}
