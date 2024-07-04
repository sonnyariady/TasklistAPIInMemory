using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TasklistAPI.Helper;
using TasklistAPI.Interface;
using TasklistAPI.Model;
using TasklistAPI.Model.Entity;
using TasklistAPI.Model.Enum;
using TasklistAPI.Model.Request;
using TasklistAPI.Model.Response;

namespace TasklistAPI.Services
{
    public class UserServices : IUserServices
    {
        private readonly AppDbContext _context;

        public UserServices(AppDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<GlobalResponse> Login(LoginRequest input)
        {
            GlobalResponse globalres = new GlobalResponse();
            bool IsValid = true;
            try
            {
                UserAccount? user = await _context.UserAccounts.Where(a => a.UserName == input.Email).FirstOrDefaultAsync();

                if (user == null)
                {
                    globalres.status_code = HttpResponseCode.ResponseError;
                    IsValid = false;
                    globalres.message = "Email is not found";
                }
                else
                {
                    HashSalt hashSalt = HashSalt.GenerateSaltedHash(64, input.Password);
                    var Pwd = hashSalt.Hash;
                    var Salt = hashSalt.Salt;

                    var isVerifyPassword = HashSalt.VerifyPassword(input.Password, user.Password, user.PasswordSalt);

                    if (!isVerifyPassword)
                    {
                        globalres.status_code = HttpResponseCode.ResponseError;
                        IsValid = false;
                        globalres.message = "Invalid password";
                    }

                    if (IsValid)
                    {

                     var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, ConfigJwt.Subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Email", user.UserName)
                   };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigJwt.Key));

                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                           ConfigJwt.Issuer,
                           ConfigJwt.Audience,
                            claims,
                            expires: DateTime.UtcNow.AddDays(1),
                            signingCredentials: signIn);

                        LoginResult loginResult = new LoginResult();
                        loginResult.Email = user.UserName;
                        loginResult.token = new JwtSecurityTokenHandler().WriteToken(token);
                        globalres.data = loginResult;
                        globalres.status_code = HttpResponseCode.ResponseOK;
                    }
                }

            }
            catch (Exception ex)
            {
                globalres.status_code = HttpResponseCode.ResponseError;
                globalres.message = ex.Message;
            }
            return globalres;
        }
    }
}
