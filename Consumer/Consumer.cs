using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Consumer
{
    class Consumer
    {
        public ConfigurationParameters ConsumerConfig { get; set; }
        public SharedMemoryAccessor ConsumerAccessor { get; set; }
        public Logger ConsumerLogger { get; set; }
        public Consumer()
        {
            var configuration = Configuration.CreateConfiguration();
            ConsumerConfig = configuration.ConsumerConfiguration;
            ConsumerLogger = new Logger(true);
            ConsumerAccessor = new SharedMemoryAccessor(ConsumerConfig, ReadSharedMemory, ConsumerLogger);
            
        }
        public void Start()
        {
            ConsumerAccessor.Start();
        }

        public static void ReadSharedMemory(SharedMemoryAccessor accessor, Logger logger)
        {
            var transaction = accessor.GetTransaction();
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
            logger.Log("Decrypted data " + Helper.ByteArrayToString(decryptedData));
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


