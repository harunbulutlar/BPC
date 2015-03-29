using System;
using System.Text;
using Utility;

namespace Producer
{
    class Program
    {
        private static void Main()
        {
            var configuration = Configuration.CreateConfiguration();
            var memoryAccess = new SharedMemoryAccessor(configuration.ProducerConfiguration, WriteToSharedMemory, new Logger(true));
            memoryAccess.Start();
        }

        public static void WriteToSharedMemory(SharedMemoryAccessor accessor, Logger logger)
        {
            if (accessor.TransactionCount >= 1999)
            {
                logger.Log("Buffer is full lets wait for it");
                return;
            }
            var encryptionType = EncryptionFactory.GetRandomTrimmedEncryption();
            logger.Log("Selected random encryption identifier " + encryptionType);
            var encryption = EncryptionFactory.GetEncryption(encryptionType);
            logger.Log("Selected encryption type " + encryption);
            var rand = new Random();
            var randomBytes = new byte[8];
            rand.NextBytes(randomBytes);
            logger.Log("Encrypting bytes: " + ByteArrayToString(randomBytes));
            var encryptedBytes = encryption.Encrypt(randomBytes);
            accessor.WriteTransaction(encryptedBytes);

        }
        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }
}
