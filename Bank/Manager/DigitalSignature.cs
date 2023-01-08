using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class DigitalSignature
    {
        public enum HashAlgorithm { SHA1, SHA256 }

        public static byte[] Create(string message, X509Certificate2 certificate)
        {
            byte[] sign = null;

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PrivateKey;

            if (csp == null)
            {
                return sign;
            }

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] buffer = encoding.GetBytes(message);

            SHA1Managed sha256 = new SHA1Managed();
            byte[] hash = sha256.ComputeHash(buffer);

            sign = csp.SignHash(hash, CryptoConfig.MapNameToOID(HashAlgorithm.SHA1.ToString()));

            return sign;
        }

        public static bool Verify(string message, byte[] signature, X509Certificate2 certificate)
        {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] buffer = encoding.GetBytes(message);

            SHA1Managed sha256 = new SHA1Managed();
            byte[] hash = sha256.ComputeHash(buffer);

            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID(HashAlgorithm.SHA1.ToString()), signature);
        }
    }
}
