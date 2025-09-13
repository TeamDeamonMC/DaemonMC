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
            byte[] AesKey = Convert.FromBase64String("hu/17aKc6oYBuN3e53k/y1AvEqI5tKjrZvpqU1U3lbA=");

            Console.WriteLine($"AES Key: {BitConverter.ToString(AesKey)}");

            Encryptor encryptor = new Encryptor(AesKey);

            Console.WriteLine("\nmessage1 decrypt"); //counter 0
            byte[] decrypted = encryptor.Decrypt(new byte[] { 0x31, 0x14, 0x17, 0x71, 0x93, 0xfe, 0xe6, 0x20, 0xff, 0x65, 0x5d });
            Console.WriteLine($"Decrypted: {BitConverter.ToString(decrypted)}");
            Console.WriteLine($"Result: {(Enumerable.SequenceEqual(decrypted, new byte[] { 0xFF, 0x01, 0x04 }) ? "VALID" : "INVALID")}");

            Console.WriteLine("\nmessage2 decrypt"); //counter 1
            byte[] decrypted2 = encryptor.Decrypt(new byte[] { 0x66, 0x5c, 0xa7, 0x00, 0xd0, 0x9d, 0x56, 0x5c, 0x34, 0x17, 0x3c, 0x54, 0x86 });
            Console.WriteLine($"Decrypted: {BitConverter.ToString(decrypted2)}");
            Console.WriteLine($"Result: {(Enumerable.SequenceEqual(decrypted2, new byte[] { 0xff, 0x03, 0x81, 0x01, 0x01 }) ? "VALID" : "INVALID")}");

            Console.WriteLine("\nmessage1 encrypt"); //counter 0
            byte[] encrypted = encryptor.Encrypt(new byte[] { 0x00, 0x00, 0x1e, 0x00, 0xe1, 0xff, 0x05, 0x02, 0x00, 0x00, 0x00, 0x00, 0x17, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x01, 0x00, 0x00, 0xff, 0xff });
            Console.WriteLine($"Encrypted: {BitConverter.ToString(encrypted)}");
            Console.WriteLine($"Result: {(Enumerable.SequenceEqual(encrypted, new byte[] { 0xCE, 0x15, 0x0D, 0x9F, 0xAE, 0x5F, 0x0B, 0x74, 0x17, 0x53, 0xBD, 0x99, 0x48, 0x20, 0x01, 0xD1, 0x8A, 0xFB, 0xE3, 0xA0, 0x10, 0x22, 0x48, 0x00, 0x93, 0x4B, 0xD6, 0xC7, 0xE7, 0xEA, 0x39, 0x3B, 0x97, 0xDB, 0x08, 0x32, 0x83, 0x30, 0x1C, 0xCE, 0xAD, 0x3B, 0xE1, 0xAE, 0x71, 0xFB, 0x12, 0x6A, 0xBC, 0xB7, 0x65, 0x09, 0xAE, 0xA3 }) ? "VALID" : "INVALID")}");

            Console.WriteLine("\nmessage2 encrypt"); //counter 1
            byte[] encrypted2 = encryptor.Encrypt(new byte[] { 0x00, 0x00, 0x13, 0x00, 0xEC, 0xFF, 0x12, 0x07, 0x00, 0x00, 0x00, 0x07, 0x31, 0x2E, 0x32, 0x31, 0x2E, 0x37, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0xFF, 0xFF });
            Console.WriteLine($"Encrypted: {BitConverter.ToString(encrypted2)}");
            Console.WriteLine($"Result: {(Enumerable.SequenceEqual(encrypted2, new byte[] { 0x84, 0xDD, 0x60, 0xD8, 0x23, 0x05, 0xD2, 0xBC, 0x4D, 0xF4, 0x40, 0xBF, 0x98, 0x85, 0xEF, 0x71, 0x34, 0xA4, 0xA3, 0x66, 0x97, 0x76, 0x3B, 0x71, 0x07, 0xC2, 0x68, 0xD9, 0x0E, 0x4F, 0x99, 0xC7, 0xE6, 0x0D, 0x04, 0xD6, 0x68, 0x88, 0x82, 0x2A, 0x72, 0x9F, 0x54 }) ? "VALID" : "INVALID")}");
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

            Encryptor encryptor = new Encryptor(aesKey);

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
