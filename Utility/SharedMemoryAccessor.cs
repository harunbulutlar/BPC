using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace Utility
{
    public class SharedMemoryAccessor
    {
        public ConfigurationParameters CurrentConfiguration { get; private set; }
        public SharedMemoryMethod Method { get; private set; }
        public Logger CurrentLogger { get; private set; }
        public delegate void SharedMemoryMethod(SharedMemoryAccessor mmf, Logger logger);

        public MemoryMappedFile SharedMemory { get; set; }
        public MemoryMappedViewAccessor SharedMemoryView { get; set; }
        public const int c_TransactionSize = 256;
        public SharedMemoryAccessor(ConfigurationParameters configuration, SharedMemoryMethod method, Logger logger)
        {
            CurrentConfiguration = configuration;
            Method = method;
            CurrentLogger = logger;
            SharedMemory = MemoryMappedFile.CreateOrOpen("SharedMemory", c_TransactionSize*2000);
            SharedMemoryView = SharedMemory.CreateViewAccessor();
        }


        public int TransactionCount
        {
            get
            {
                short count;
                SharedMemoryView.Read(0, out count);
                CurrentLogger.Log("element count " + count);
                return count;
            }
            set
            {
                SharedMemoryView.Write(0, (short)value);
                CurrentLogger.Log("element count set to " + value);
            }
        }

        public void Start()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var delay = CurrentConfiguration.RandomDelay;
            CurrentLogger.Log("has delay " + delay);
            var semaphore = new Semaphore(1, 1,"LetsTrysemaphore");
            while (true)
            {
                CurrentLogger.Log("Waiting for handle");
                semaphore.WaitOne();
                CurrentLogger.Log("handle received");
                Thread.Sleep(delay);
                var methodTime =Stopwatch.StartNew();
                //Call the callback method
                Method(this, CurrentLogger);
                methodTime.Stop();
                CurrentLogger.Log("Method Execution Time " +methodTime.ElapsedMilliseconds);

                semaphore.Release();
                if (stopWatch.Elapsed.TotalMilliseconds > CurrentConfiguration.AliveTime)
                {
                    CurrentLogger.Log("finished");
                    break;
                }
            }

        }

        public void WriteTransaction(byte[] data)
        {
            var transactionCount = TransactionCount;
            var transaction = new byte[c_TransactionSize];
            data.CopyTo(transaction, 0);
            SharedMemoryView.WriteArray(transactionCount * c_TransactionSize + 2, transaction, 0, transaction.Length);
            transactionCount++;
            TransactionCount = transactionCount;
        }

        public byte[] GetTransaction()
        {
            var elementCount = TransactionCount;
            var arr = new byte[c_TransactionSize];
            if (elementCount == 0)
            {
                CurrentLogger.Log("Shared memory is empty");
                return null;
            }

            SharedMemoryView.ReadArray((elementCount - 1) * c_TransactionSize + 2, arr, 0, c_TransactionSize);
            TransactionCount = elementCount - 1;
            return arr;
        }

    }
}
 