using System;
using System.Linq;
using System.Text;
using Cont5.Models.Login;
using Cont5.Models.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Cont5.Helpers;
using System.Security.Claims;
using System.Reflection;

namespace Cont5.Services {
    public interface ILoginService {
        AuthenticateModel Authenticate(AuthenticateRequest request);
        RenewTokenModel RenewToken();
        HashPasswordModel HashPassword(HashPasswordRequest request);
        GetRoleIdModel GetRoleId();
        ValidateTokenModel ValidateToken(ValidateTokenRequest request);
    }

    public class LoginService : ILoginService {
        private readonly IConfigHelper configHelper;
        private readonly IUserAccountHelper userAccountHelper;
        private readonly IEncryptionHelper encryptionHelper;
        private readonly IJsonHelper jsonHelper;

        public LoginService(IConfigHelper configHelper, IUserAccountHelper userAccountHelper, IEncryptionHelper encryptionHelper, IJsonHelper jsonHelper) {
            this.configHelper = configHelper;
            this.userAccountHelper = userAccountHelper;
            this.encryptionHelper = encryptionHelper;
            this.jsonHelper = jsonHelper;
        }

        public AuthenticateModel Authenticate(AuthenticateRequest request) {
            var loginFailedList = jsonHelper.AuditList.Where(w => w.Api == "Authenticate" && w.CreatedDt >= DateTime.Today && w.Response != "Login Successful").GroupBy(g => g.ClientIp).Where(w => w.Count() >= configHelper.BlockIpLoginAttempts).Select(s => s.Key).Distinct().ToList();
            var clientIp = userAccountHelper.GetClientIp();
            var result = new AuthenticateModel();

            if (loginFailedList.Contains(clientIp)) {
                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Request = $"UserName: {request.UserName}",
                    Response = "Login Failed - ip blocked",
                    Exception = null,
                    ClientIp = clientIp,
                    CreatedBy = null,
                    CreatedDt = DateTime.Now,
                });
                return result;
            }
            
            var loginAccount = userAccountHelper.AccountList;
            var user = loginAccount.SingleOrDefault(s => s.UserName == request.UserName);

            if (user == null) {
                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Request = $"UserName: {request.UserName}",
                    Response = "Login Failed - invalid user",
                    Exception = null,
                    ClientIp = clientIp,
                    CreatedBy = null,
                    CreatedDt = DateTime.Now,
                });
                return result;
            }
            var hashModel = new HashPasswordRequest {
                Salt = user.Salt,
                Password = request.Password,
            };
            var hashedPw = HashPassword(hashModel);

            if (hashedPw.HashedPassword == user.Password) {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configHelper.JwtSecret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] 
                    {
                        new Claim("id", user.UserId.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                result.Token = tokenHandler.WriteToken(token);

                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Request = $"UserName: {request.UserName}",
                    Response = "Login Successful",
                    Exception = null,
                    ClientIp = clientIp,
                    CreatedBy = null,
                    CreatedDt = DateTime.Now,
                });
            } else {
                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Request = $"UserName: {request.UserName}",
                    Response = "Login Failed - invalid password",
                    Exception = null,
                    ClientIp = clientIp,
                    CreatedBy = null,
                    CreatedDt = DateTime.Now,
                });
            }

            return result;
        }

        public HashPasswordModel HashPassword(HashPasswordRequest request) {
            byte[] salt = new byte[128 / 8];
            if (String.IsNullOrWhiteSpace(request.Salt)) {
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            }
            else {
                salt = Convert.FromBase64String(request.Salt);
            }
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            
            return new HashPasswordModel(Convert.ToBase64String(salt), hashed);
        }

        public RenewTokenModel RenewToken() {
            var result = new RenewTokenModel() {
                Token = userAccountHelper.GenerateTempToken(3, 0)
            };

            return result;
        }

        public GetRoleIdModel GetRoleId() {
            var result = new GetRoleIdModel(){
                RoleId = userAccountHelper.CurrentLogin.RoleId,
            };

            return result;
        }
    
        public ValidateTokenModel ValidateToken(ValidateTokenRequest request) {
            return new ValidateTokenModel{ Result = (this.userAccountHelper.ValidateToken(request.Token) != null)};
        }
    }
}