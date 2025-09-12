using System.Security.Cryptography;
using System.Text;
using DaemonMC.Utils;

namespace Test
{
    [TestClass]
    public class CryptoTest
    {
        [TestMethod]
        public void TestWithReferenceValues()
        {
            byte[] AesKey = Convert.FromBase64String("D9IydkldINN4PdCNTeivjEQGZ5AFVFPlDqxJyUHZl8E=");
            byte[] Salt = Encoding.UTF8.GetBytes("");

            Console.WriteLine($"AES Key: {BitConverter.ToString(AesKey)}");
            Console.WriteLine($"Salt: {BitConverter.ToString(Salt)}");

            Encryptor encryptor = new Encryptor(AesKey, AesKey);

            Console.WriteLine("\nmessage1");
            byte[] decrypted = encryptor.Decrypt(new byte[] { 0x97, 0xBC, 0xF2, 0x74, 0x7b, 0x63, 0x75, 0x1e, 0x39, 0xbe, 0x21 });
            Console.WriteLine($"Decrypted: {BitConverter.ToString(decrypted)}");
            Console.WriteLine($"Result: {(Enumerable.SequenceEqual(decrypted, new byte[] { 0xFF, 0x01, 0x04 }) ? "VALID" : "INVALID")}");

            Console.WriteLine("\nmessage2");
            byte[] decrypted2 = encryptor.Decrypt(new byte[] { 0xc3, 0x7e, 0x8f, 0x60, 0x53, 0x27, 0x82, 0xfc, 0x37, 0x8b, 0xa8, 0xfc, 0x07 });
            Console.WriteLine($"Decrypted: {BitConverter.ToString(decrypted2)}");
            Console.WriteLine($"Result: {(Enumerable.SequenceEqual(decrypted2, new byte[] { 0xff, 0x03, 0x81, 0x01, 0x01 }) ? "VALID" : "INVALID")}");
        }

        [TestMethod]
        public void CheckEncryptor()
        {
            using var ecdhLocal = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
            using var ecdhRemote = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);

            byte[] ecdhSharedSecret = ecdhLocal.DeriveKeyMaterial(ecdhRemote.PublicKey);
            Console.WriteLine($"Generated ECDH Shared Secret: {BitConverter.ToString(ecdhSharedSecret)} ({ecdhSharedSecret.Length} bytes)");

            byte[] secretPrepend = new byte[16];
            RandomNumberGenerator.Fill(secretPrepend);
            Console.WriteLine($"Generated Salt: {BitConverter.ToString(secretPrepend)}");

            using var sha256 = SHA256.Create();
            byte[] combined = new byte[secretPrepend.Length + ecdhSharedSecret.Length];
            Buffer.BlockCopy(secretPrepend, 0, combined, 0, secretPrepend.Length);
            Buffer.BlockCopy(ecdhSharedSecret, 0, combined, secretPrepend.Length, ecdhSharedSecret.Length);

            byte[] aesKey = sha256.ComputeHash(combined);
            Console.WriteLine($"Generated AES Key: {BitConverter.ToString(aesKey)} ({aesKey.Length} bytes)");

            byte[] message = Encoding.UTF8.GetBytes("Hi, if you can read this, encryptor and decryptor works!!!");
            Console.WriteLine($"Test Message: {Encoding.UTF8.GetString(message)}");
            Console.WriteLine($"Test Message Bytes: {BitConverter.ToString(message)}");

            Encryptor encryptor = new Encryptor(aesKey, ecdhSharedSecret);

            byte[] encrypted = encryptor.Encrypt(message);
            Console.WriteLine($"Encrypted Data: {BitConverter.ToString(encrypted)}");

            byte[] decrypted = encryptor.Decrypt(encrypted);
            Console.WriteLine($"Decrypted Data: {BitConverter.ToString(decrypted)}");

            string decryptedText = Encoding.UTF8.GetString(decrypted);
            Console.WriteLine($"Decrypted Text: {decryptedText}");

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
