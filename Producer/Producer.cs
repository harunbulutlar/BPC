using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Producer
{
    class Producer
    {
        public ConfigurationParameters ProducerConfig { get; set; }
        public SharedMemoryAccessor ProducerAccessor { get; set; }
        public Logger ProducerLogger { get; set; }
        public Producer()
        {
            var configuration = Configuration.CreateConfiguration();
            ProducerConfig = configuration.ProducerConfiguration;
            ProducerLogger = new Logger(true);
            ProducerAccessor = new SharedMemoryAccessor(ProducerConfig, WriteToSharedMemory, ProducerLogger);

        }
        public void Start()
        {
            ProducerAccessor.Start();
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
            logger.Log("Encrypting bytes: " + Helper.ByteArrayToString(randomBytes));
            var encryptedBytes = encryption.Encrypt(randomBytes);
            accessor.WriteTransaction(encryptedBytes);

        }

    }
}


