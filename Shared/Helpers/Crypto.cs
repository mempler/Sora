#region copyright

/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Shared.Helpers
{
    public static class Crypto
    {
        public static string RandomString(int n)
        {
            StringBuilder ret   = new StringBuilder();
            const string  ascii = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/=";

            for (int i = 0; i < n; i++)
                ret.Append(ascii[new Random().Next(0, ascii.Length)]);

            return ret.ToString();
        }

        public static string EncryptString(string message, byte[] key, ref string iv)
        {
            byte[] rawMessage = Encoding.ASCII.GetBytes(message);

            byte[] newiv = iv == null ? Encoding.ASCII.GetBytes(RandomString(32)) : Convert.FromBase64String(iv);

            RijndaelEngine            engine         = new RijndaelEngine(256);
            CbcBlockCipher            blockCipher    = new CbcBlockCipher(engine);
            PaddedBufferedBlockCipher cipher         = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            KeyParameter              keyParam       = new KeyParameter(key);
            ParametersWithIV          keyParamWithIv = new ParametersWithIV(keyParam, newiv, 0, 32);

            cipher.Init(true, keyParamWithIv);
            byte[] comparisonBytes = new byte[cipher.GetOutputSize(rawMessage.Length)];
            int length          = cipher.ProcessBytes(rawMessage, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            iv = Convert.ToBase64String(newiv);

            return Convert.ToBase64String(comparisonBytes);
        }

        public static string DecryptString(byte[] message, byte[] key, byte[] iv)
        {            
            RijndaelEngine            engine         = new RijndaelEngine(256);
            CbcBlockCipher            blockCipher    = new CbcBlockCipher(engine);
            PaddedBufferedBlockCipher cipher         = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            KeyParameter              keyParam       = new KeyParameter(key);
            ParametersWithIV          keyParamWithIv = new ParametersWithIV(keyParam, iv, 0, 32);
            
            cipher.Init(false, keyParamWithIv);
            byte[] comparisonBytes = new byte[cipher.GetOutputSize(message.Length)];
            int    length          = cipher.ProcessBytes(message, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            return Encoding.UTF8.GetString(comparisonBytes);
        }

        public static byte[] GetMd5(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
                return md5.ComputeHash(data);
        }

        public static byte[] GetMd5(Stream data)
        {
            using (MD5 md5 = MD5.Create())
                return md5.ComputeHash(data);
        }

        public static byte[] GetMd5(string data) => GetMd5(Encoding.UTF8.GetBytes(data));
    }
}