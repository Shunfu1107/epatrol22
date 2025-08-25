using System.Security.Cryptography;
using System.Text;

namespace AdminPortalV8.Services
{
    public interface IEncryption
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class EncryptionService : IEncryption
    {
        private readonly string _passPhrase = "mquest0509";
        private readonly string _saltValue = "@ClUb$$$$$";
        private readonly string _hashAlgorithm = "SHA1";
        private readonly int _passwordIterations = 1;
        private readonly string _initVector = "@1214EIshIEwgNis";
        private readonly int _keySize = 256;

        public string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(_saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using var password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
            byte[] keyBytes = password.GetBytes(_keySize / 8);

            using var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
            using ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                // Check if the cipherText is a valid Base64 string
                if (string.IsNullOrEmpty(cipherText) || !IsBase64String(cipherText))
                {
                    throw new FormatException("The cipherText is not a valid Base64 string.");
                }

                // Ensure padding is correct for Base64
                cipherText = FixBase64Padding(cipherText);

                byte[] initVectorBytes = Encoding.ASCII.GetBytes(_initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(_saltValue);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                using var password = new PasswordDeriveBytes(_passPhrase, saltValueBytes, _hashAlgorithm, _passwordIterations);
                byte[] keyBytes = password.GetBytes(_keySize / 8);

                using var symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC };
                using ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                using var memoryStream = new MemoryStream(cipherTextBytes);
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed: " + ex.Message);
            }
        }

        // Helper method to fix Base64 padding if it's incorrect
        private string FixBase64Padding(string cipherText)
        {
            int paddingRequired = 4 - (cipherText.Length % 4);
            if (paddingRequired != 4)
            {
                cipherText = cipherText.PadRight(cipherText.Length + paddingRequired, '=');
            }
            return cipherText;
        }


        // Helper method to check if a string is a valid Base64
        private bool IsBase64String(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            str = str.Trim();

            // Check if length is a multiple of 4 (Base64 requirement)
            if (str.Length % 4 != 0) return false;

            // Check for Base64 characters
            return str.All(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".Contains(c));
        }
    }
}
