using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using scr = Org.BouncyCastle.Crypto.Generators.SCrypt;

namespace Sora.Framework.Utilities
{
    public static class Crypto
    {
        public static string RandomString(int n)
        {
            var ret = new StringBuilder();
            const string ascii = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            for (var i = 0; i < n; i++)
                ret.Append(ascii[new Random().Next(0, ascii.Length)]);

            return ret.ToString();
        }

        public static byte[] PseudoSecureBytes(int n)
        {
            var provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[n];
            provider.GetBytes(byteArray);
            return byteArray;
        }

        public static string EncryptString(string message, byte[] key, ref string iv)
        {
            var rawMessage = Encoding.ASCII.GetBytes(message);

            var newiv = iv == null ? Encoding.ASCII.GetBytes(RandomString(32)) : Convert.FromBase64String(iv);

            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var keyParam = new KeyParameter(key);
            var keyParamWithIv = new ParametersWithIV(keyParam, newiv, 0, 32);

            cipher.Init(true, keyParamWithIv);
            var comparisonBytes = new byte[cipher.GetOutputSize(rawMessage.Length)];
            var length = cipher.ProcessBytes(rawMessage, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            iv = Convert.ToBase64String(newiv);

            return Convert.ToBase64String(comparisonBytes);
        }

        public static string DecryptString(byte[] message, byte[] key, byte[] iv)
        {
            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var keyParam = new KeyParameter(key);
            var keyParamWithIv = new ParametersWithIV(keyParam, iv, 0, 32);

            cipher.Init(false, keyParamWithIv);
            var comparisonBytes = new byte[cipher.GetOutputSize(message.Length)];
            var length = cipher.ProcessBytes(message, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            return Encoding.UTF8.GetString(comparisonBytes);
        }

        public static byte[] GetMd5(byte[] data)
        {
            using (var md5 = MD5.Create())
                return md5.ComputeHash(data);
        }

        public static byte[] GetMd5(Stream data)
        {
            using (var md5 = MD5.Create())
                return md5.ComputeHash(data);
        }

        public static byte[] GetMd5(string data) => GetMd5(Encoding.UTF8.GetBytes(data));

        public static class BCrypt {
            /// <summary>
            /// Generate BCrypt Hashed Password. this is really cpu heavy which means this should be run only once per user!
            /// </summary>
            /// <param name="password">Password Md5 String which is required to verify the Password</param>
            /// <param name="rounds">How much the Hash should be rounded.</param>
            /// <returns></returns>
            [DllImport(@"lib/BCrypt")]
            public static extern string generate_hash(string password, int rounds = 12);

            /// <summary>
            /// Validate if a password is equal to Password BCrypt Hash
            /// </summary>
            /// <param name="password">Password Md5</param>
            /// <param name="hash">Password BCrypt Hash</param>
            /// <returns></returns>
            [DllImport(@"lib/BCrypt")]
            public static extern bool validate_password(string password, string hash);
        }

        public static class SCrypt
        {
            public static byte[] generate_salt()
            {
                return PseudoSecureBytes(new Random().Next(90,100));
            }
            
            public static (string password, byte[] salt) generate_hash(string password, int rounds = 20)
            {
                var pwBytes = Hex.IsHex(password) ? Hex.FromHex(password) : Encoding.Default.GetBytes(password);
                var saltBytes = generate_salt();
                var pwHashBytes = scr.Generate(pwBytes, saltBytes, 262144 / 4, rounds, 1, 512);
                return (Convert.ToBase64String(pwHashBytes), saltBytes);
            }

            public static bool validate_password(string password, string hash, byte[] salt, int rounds = 20)
            {
                var pwBytes = Hex.IsHex(password) ? Hex.FromHex(password) : Encoding.Default.GetBytes(password);
                var pwHashBytes = scr.Generate(pwBytes, salt, 262144 / 4, rounds, 1, 512);
                var hashBytes = Convert.FromBase64String(hash);


                return pwHashBytes.SequenceEqual(hashBytes);
            }
        }
    }
}
