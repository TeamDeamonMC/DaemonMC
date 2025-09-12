using System.Security.Cryptography;
using DaemonMC.Network;
using DaemonMC.Utils.Text;

namespace DaemonMC.Utils
{
    public class Cryptography
    {
        public static bool UseEncryption = false; //here to enable encryption
        public static void verifyKeyInfo(byte[] data, string identityPublicKey)
        {
            using (ECDsa ecdsa = ECDsa.Create())
            {
                ecdsa.ImportSubjectPublicKeyInfo(data, out _);
                ECParameters ecParams = ecdsa.ExportParameters(false);
                string result = ecParams.Curve.Oid.Value == "1.3.132.0.34" ? "OK (secp384r1)" : "INVALID";

                Log.debug($"KeyInfo: {result} {ecParams.Curve.Oid.Value}", result == "INVALID" ? ConsoleColor.Red : ConsoleColor.Gray);
            }

            using (var ecdhRemote = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384))
            {
                ecdhRemote.ImportSubjectPublicKeyInfo(data, out int bytesRead);
                byte[] reencodedPublicKey = ecdhRemote.ExportSubjectPublicKeyInfo();
                string b64ReencodedPublicKey = Convert.ToBase64String(reencodedPublicKey);

                string result = identityPublicKey == b64ReencodedPublicKey ? "OK" : "INVALID";

                Log.debug($"IdentityPublicKey: {result} {identityPublicKey}", result == "INVALID" ? ConsoleColor.Red : ConsoleColor.Gray);
            }
        }

        public static void verifySharedSecret(byte[] data)
        {
            string result = data.Length == 32 ? "OK" : "INVALID";

            Log.debug($"SharedSecret: {result} {Convert.ToBase64String(data)}", result == "INVALID" ? ConsoleColor.Red : ConsoleColor.Gray);
        }

        public static void verifyAESKey(byte[] data)
        {
            string result = data.Length == 32 ? "OK" : "INVALID";

            Log.debug($"AESKey: {result} {Convert.ToBase64String(data)}", result == "INVALID" ? ConsoleColor.Red : ConsoleColor.Gray);
        }

        public static byte[] FixDSize(byte[] data, int targetSize)
        {
            if (data == null)
                return new byte[targetSize];

            if (data.Length == targetSize)
                return data;

            byte[] result = new byte[targetSize];

            if (data.Length > targetSize)
            {
                Buffer.BlockCopy(data, data.Length - targetSize, result, 0, targetSize);
            }
            else
            {
                Buffer.BlockCopy(data, 0, result, targetSize - data.Length, data.Length);
            }

            return result;
        }
    }

    public class Encryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _ivBase;
        private readonly IncrementalAesCtrTransform _encryptor;
        private readonly IncrementalAesCtrTransform _decryptor;
        private ulong _encryptCounter;
        private ulong _decryptCounter;
        public bool validated;

        public Encryptor(byte[] key, byte[] ecdhSharedSecret)
        {
            _key = key;

            _ivBase = ecdhSharedSecret.Take(12).Concat(new byte[] { 0, 0, 0, 2 }).ToArray();

            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                _encryptor = new IncrementalAesCtrTransform(aes, (byte[])_ivBase.Clone());
                _decryptor = new IncrementalAesCtrTransform(aes, (byte[])_ivBase.Clone());
            }
        }

        public void Validate(PacketDecoder decoder)
        {
            var expectedHandshake = new byte[] { 0xFF, 0x01, 0x04 };
            var receivedHandshake = Decrypt(decoder.buffer);

            if (Enumerable.SequenceEqual(receivedHandshake, expectedHandshake))
            {
                validated = true;
                Log.debug($"Established encrypted connection with {decoder.clientEp}");
            }
            else
            {
                Log.error($"Encryption with {decoder.clientEp} validation FAILED. Possibly key mismatch.");
            }
        }

        public byte[] Encrypt(byte[] payload)
        {
            byte[] checksum = CalculateChecksum(_encryptCounter, payload);

            byte[] fullPayload = payload.Concat(checksum).ToArray();

            byte[] encrypted = _encryptor.Transform(fullPayload);

            _encryptCounter++;
            return encrypted;
        }

        public byte[] Decrypt(byte[] encrypted)
        {
            byte[] fullPayload = _decryptor.Transform(encrypted);

            if (fullPayload.Length < 8) { Log.error($"Encrypted packet too short {BitConverter.ToString(fullPayload)}"); }

            byte[] payload = fullPayload.Take(fullPayload.Length - 8).ToArray();
            byte[] actualChecksum = fullPayload.Skip(fullPayload.Length - 8).ToArray();
            Log.debug($"AES Key: {BitConverter.ToString(_key)}");
            Log.debug($"IV: {BitConverter.ToString(_ivBase)}");
            byte[] expectedChecksum = CalculateChecksum(_decryptCounter, payload);

            if (!expectedChecksum.SequenceEqual(actualChecksum)) { Log.error($"Checksum failed at counter {_decryptCounter}. Expected {BitConverter.ToString(expectedChecksum)} got {BitConverter.ToString(actualChecksum)}"); }

            _decryptCounter++;
            return payload;
        }

        private byte[] CalculateChecksum(ulong counter, byte[] payload)
        {
            using var sha256 = SHA256.Create();

            byte[] counterBytes = BitConverter.GetBytes(counter);

            sha256.TransformBlock(counterBytes, 0, counterBytes.Length, null, 0);
            sha256.TransformBlock(payload, 0, payload.Length, null, 0);
            sha256.TransformFinalBlock(_key, 0, _key.Length);

            return sha256.Hash.Take(8).ToArray();
        }
    }

    public class IncrementalAesCtrTransform : IDisposable
    {
        private readonly ICryptoTransform _aesTransform;
        private readonly byte[] _counter;
        private readonly byte[] _counterBlock;
        private readonly byte[] _keystream;
        private int _keystreamIndex;

        public IncrementalAesCtrTransform(Aes aes, byte[] iv)
        {
            _aesTransform = aes.CreateEncryptor(aes.Key, new byte[16]);
            _counter = (byte[])iv.Clone();
            _counterBlock = new byte[16];
            _keystream = new byte[16];
            _keystreamIndex = 16;
        }

        public byte[] Transform(byte[] input)
        {
            byte[] output = new byte[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                if (_keystreamIndex == 16)
                {
                    EncryptCounter();
                    _keystreamIndex = 0;
                }

                output[i] = (byte)(input[i] ^ _keystream[_keystreamIndex++]);
            }

            return output;
        }

        private void EncryptCounter()
        {
            Buffer.BlockCopy(_counter, 0, _counterBlock, 0, 16);
            _aesTransform.TransformBlock(_counterBlock, 0, 16, _keystream, 0);
            IncrementCounter();
        }

        private void IncrementCounter()
        {
            for (int i = 15; i >= 12; i--)
            {
                if (++_counter[i] != 0) break;
            }
        }

        public void Dispose() => _aesTransform.Dispose();
    }
}