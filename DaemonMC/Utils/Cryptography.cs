using System.Security.Cryptography;
using DaemonMC.Utils.Text;
using DaemonMC.Network;

namespace DaemonMC.Utils
{
    public class Cryptography
    {
        public static bool UseEncryption = false; //here to enable encryption
        public static void verifyKeyInfo(byte[] data)
        {
            using (ECDsa ecdsa = ECDsa.Create())
            {
                ecdsa.ImportSubjectPublicKeyInfo(data, out _);

                ECParameters ecParams = ecdsa.ExportParameters(false);

                string result = ecParams.Curve.Oid.Value == "1.3.132.0.34" ? "OK (secp384r1)" : "INVALID";

                Log.debug($"KeyInfo: {result} {ecParams.Curve.Oid.Value}");
            }
        }
    }

    public class Encryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        public bool validated;

        public Encryptor(byte[] key)
        {
            _key = key;
            _iv = key.Take(12).Concat(new byte[] { 0, 0, 0, 2 }).ToArray();
        }

        public byte[] Encrypt(byte[] plaintext) => Process(plaintext, true);
        public byte[] Decrypt(byte[] ciphertext) => Process(ciphertext, false);

        private byte[] Process(byte[] input, bool encrypting)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using var transform = new IncrementalAesCtrTransform(aes);
                return transform.Transform(input);
            }
        }

        public void Validate(PacketDecoder decoder)
        {
            var expectedHandshake = new byte[] { 0xFF, 0x01, 0x04 };
            var receivedHandshake = Decrypt(decoder.buffer.Skip(1).ToArray().Take(decoder.buffer.Length - 9).ToArray());

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
    }

    public class IncrementalAesCtrTransform : IDisposable
    {
        private readonly ICryptoTransform _aesTransform;
        private readonly byte[] _counter;
        private readonly byte[] _counterBlock;
        private readonly byte[] _keystream;
        private int _keystreamIndex;

        public IncrementalAesCtrTransform(Aes aes)
        {
            _aesTransform = aes.CreateEncryptor();
            _counter = (byte[])aes.IV.Clone();
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
            Buffer.BlockCopy(_counter, 0, _counterBlock, 0, _counter.Length);
            _aesTransform.TransformBlock(_counterBlock, 0, 16, _keystream, 0);
            IncrementCounter();
        }

        private void IncrementCounter()
        {
            for (int i = 15; i >= 0; i--)
            {
                if (++_counter[i] != 0) break;
            }
        }

        public void Dispose() => _aesTransform.Dispose();
    }
}
