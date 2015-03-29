using System;
using System.Text;
using Utility;

namespace Consumer
{
    class Program
    {
        private static void Main()
        {
            var configuration = Configuration.CreateConfiguration();
            var memoryAccess = new SharedMemoryAccessor(configuration.ConsumerConfiguration, ReadSharedMemory, new Logger(true));
            memoryAccess.Start();
        }

        public static void ReadSharedMemory(SharedMemoryAccessor accessor, Logger logger)
        {
            var transaction =  accessor.GetTransaction();
            if (transaction == null)
            {
                return;
            }
            var trimmedTypeBytes = new byte[2];
            Buffer.BlockCopy(transaction, 2, trimmedTypeBytes, 0, 2);
            var trimmedType = Encoding.Default.GetString(trimmedTypeBytes);
            var encryption = EncryptionFactory.GetEncryption(trimmedType);
            logger.Log("Encryption Type " + encryption);
            var decryptedData = encryption.Decrypt(transaction);
            logger.Log("Decrypted data " + ByteArrayToString(decryptedData));
        }
        public static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
