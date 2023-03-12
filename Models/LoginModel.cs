namespace Cont5.Models.Login {
    public class LoginAccount {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public LoginAccount(int userId, int roleId, string userName, string password, string salt) {
            UserId = userId;
            RoleId = roleId;
            UserName = userName;
            Password = password;
            Salt = salt;
        }
    }

    public class AuthenticateRequest {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticateModel {
        public string Token { get; set; }
    }

    public class HashPasswordRequest {
        public string Password { get; set; }
        public string Salt { get; set; }
    }

    public class HashPasswordModel {
        public string Salt { get; set; }
        public string HashedPassword { get; set; }

        public HashPasswordModel(string salt, string hashedPassword) {
            Salt = salt;
            HashedPassword = hashedPassword;
        }
    }

    public class CurrentLoginModel : LoginAccount {
        public string Token { get; set; }
        public CurrentLoginModel(LoginAccount loginAccount): base(loginAccount.UserId, loginAccount.RoleId, loginAccount.UserName, loginAccount.Password, loginAccount.Salt) { }
    }
    
    public class EncryptTokenModel {
        public string Token { get; set; }
    }

    public class EncryptTokenRequest {
        public string Token { get; set; }
    }

    public class RenewTokenModel {
        public string Token { get; set; }
    }

    public class GetRoleIdModel {
        public int RoleId { get; set; }
    }

    public class ValidateTokenRequest {
        public string Token { get; set; }
    }
    public class ValidateTokenModel {
        public bool Result { get; set; }
    }
}