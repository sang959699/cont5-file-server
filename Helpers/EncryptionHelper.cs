using System.Security.Cryptography;
using System;
using System.Text;

namespace Cont5.Helpers
{
    public interface IEncryptionHelper
    {
        string Encrypt (string request);
        string Decrypt (string request);
    }
    public class EncryptionHelper : IEncryptionHelper
    {
        private readonly RSA rsa;
        public EncryptionHelper(IConfigHelper configHelper) {
            rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(configHelper.RsaPublicKey), out _);
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(configHelper.RsaPrivateKey), out _);
        }
        public string Encrypt (string request) {
            var result = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(request), RSAEncryptionPadding.Pkcs1));
            return result;
        }

        public string Decrypt (string request) {
            var plainText = rsa.Decrypt(Convert.FromBase64String(request), RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(plainText);
        }
    }
}