using Cont5.Models.Login;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Cont5.Helpers
{
    public interface IUserAccountHelper
    {
        List<LoginAccount> AccountList { get; }
        CurrentLoginModel CurrentLogin { get; }
        LoginAccount ValidateToken (string token);
        string GenerateTempToken (int hours, int mins);
        string GetClientIp ();
    }
    public class UserAccountHelper : IUserAccountHelper
    {
        private readonly IConfigHelper configHelper;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserAccountHelper(IConfigHelper configHelper, IHttpContextAccessor httpContextAccessor) {
            this.configHelper = configHelper;
            this.httpContextAccessor = httpContextAccessor;
        }
        
        public List<LoginAccount> AccountList {
            get {
                var userPath = configHelper.UserJsonPath;
                var json = File.ReadAllText(userPath);

                var result = JsonConvert.DeserializeObject<List<LoginAccount>>(json);

                return result;
            }
        }

        public CurrentLoginModel CurrentLogin {
            get {
                var identity = (ClaimsIdentity) httpContextAccessor.HttpContext.User.Identity;  
                IEnumerable<Claim> claims = identity.Claims;
                var userId = Convert.ToInt16((claims.Where(p => p.Type == "id").Single().Value));
                var userAccount = AccountList.Where(w => w.UserId == userId).Single();
                var result = new CurrentLoginModel(userAccount) {
                    Token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""),
                };
                return result;
            }
        }

        public string GenerateTempToken(int hours, int mins) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configHelper.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim("id", CurrentLogin.UserId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(hours).AddMinutes(mins),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public LoginAccount ValidateToken(string token) {
            int userId = 0;
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configHelper.JwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            var handler = new JwtSecurityTokenHandler();
            try {
                var principal = handler.ValidateToken(token, validationParameters, out var validToken);
                JwtSecurityToken validJwt = validToken as JwtSecurityToken;
                var payload = validJwt.Payload.Where(w => w.Key == "id").Single();
                userId = Convert.ToInt16(payload.Value);
                if (userId == 0) throw new Exception();
            } catch (Exception) {
                return null;
            }
            return AccountList.Single(s => s.UserId == Convert.ToInt32(userId));
        }

        public string GetClientIp() {
            return httpContextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}