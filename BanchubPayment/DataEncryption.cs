using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BanchubPayment
{
    public class DataEncryption
    {
        public static string EncryptString(string inputText, char[] password)
        {
            using (RijndaelManaged RijndaelCipher = new RijndaelManaged())
            {
                byte[] PlainText = Encoding.Unicode.GetBytes(inputText);
                byte[] Salt = Encoding.ASCII.GetBytes(new string(password).Length.ToString());

                //The standard is documented in IETF RRC 2898.
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(new string(password), Salt);
                //Creates a symmetric encryptor object. 
                ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream();

                //Defines a stream that links data streams to cryptographic transformations
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(PlainText, 0, PlainText.Length);
                //Writes the final state and clears the buffer
                cryptoStream.FlushFinalBlock();
                byte[] CipherBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();

                return Convert.ToBase64String(CipherBytes);
            }
        }

        /// <summary>
        /// Method which does the encryption using Rijndeal algorithm.This is for decrypting the data
        /// which has orginally being encrypted using the above method
        /// <returns>Decrypted Data</returns>
        public static string DecryptString(string inputText, char[] password)
        {
            using (RijndaelManaged RijndaelCipher = new RijndaelManaged())
            {
                byte[] EncryptedData = Convert.FromBase64String(inputText);
                byte[] Salt = Encoding.ASCII.GetBytes(new string(password).Length.ToString());

                //Making of the key for decryption
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(new string(password), Salt);
                //Creates a symmetric Rijndael decryptor object.

                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(EncryptedData);
                //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                byte[] PlainText = new byte[EncryptedData.Length];
                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

                memoryStream.Close();
                cryptoStream.Close();

                //Converting to string
                return Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            }
        }

        public static string DecryptStringAES(string cipherText, string skey = null)
        {
            //var arr = skey.ToCharArray();
            //Array.Reverse(arr);
            //skey = (new string(arr)).PadLeft(16, '0');

            skey = skey.PadLeft(16, '0');
            var keybytes = Encoding.UTF8.GetBytes(skey.Substring(0, 16));
            var iv = Encoding.UTF8.GetBytes(skey.Substring(0, 16));

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();

                            }
                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

    }

}