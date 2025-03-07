using System.Text;
using DaemonMC;
using DaemonMC.Utils;

namespace Test
{
    [TestClass]
    public class CryptoTest
    {
        private byte[] secretKey = Convert.FromBase64String("TNT0i2IPrytrQf8huJ2VKqqq4VW2yWHl1BmGex3xWM8=");

        [TestMethod]
        public void ValidateHandshake()
        {
            byte[] encryptedData = new byte[] { 0xFE, 0xE2, 0x91, 0xE0, 0x5C, 0xFF, 0x9D, 0x09, 0x01, 0x7B, 0x9D, 0x99 };

            Encryptor encryptor = new Encryptor(secretKey);

            byte[] decrypted = encryptor.Decrypt(encryptedData.Skip(1).ToArray().Take(encryptedData.Length - 9).ToArray());
            ToDataTypes.HexDump(decrypted, decrypted.Length);

            if (Enumerable.SequenceEqual(decrypted, new byte[] { 0xFF, 0x01, 0x04 }))
            {
                Console.WriteLine("Validated OK");
            }
            else
            {
                Console.WriteLine("FAILED");
            }
        }

        [TestMethod]
        public void CheckEncryptor()
        {
            byte[] message = Encoding.UTF8.GetBytes("Hi, if you can read this, encryptor and decryptor works!!!");

            Encryptor encryptor = new Encryptor(secretKey);

            byte[] encrypted = encryptor.Encrypt(message);

            byte[] decrypted = encryptor.Decrypt(encrypted);

            Console.WriteLine(Encoding.UTF8.GetString(decrypted));

            if (Enumerable.SequenceEqual(decrypted, message))
            {
                Console.WriteLine("Validated OK");
            }
            else
            {
                Console.WriteLine("FAILED");
            }
        }
    }
}
