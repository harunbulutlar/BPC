using System.Collections.Generic;
using System.Threading;
using Utility;

namespace ProducerConsumerManager
{
    class Program
    {
        static void Main()
        {
            var logger = new Logger(true);
            var configuration = Configuration.CreateConfiguration();
            var processes = new List<AbstractProcessHost>();

            //Create Consumers
            for (var i = 0; i < configuration.ConsumerConfiguration.NumberOfProcesses; i++)
            {
                var consumer = new ConsumerHost();
                if (!consumer.Start()) continue;
                processes.Add(consumer);
            }
            //Create Producers
            for (var i = 0; i < configuration.ProducerConfiguration.NumberOfProcesses; i++)
            {
                var producer = new ProducerHost();
                if (producer.Start()) continue;
                processes.Add(producer);
            }

            //Wait for all of them to exit
            foreach (var process in processes)
            {
                process.Close();
            }

            logger.Log("Finished all process waiting to exiting after 10 seconds");
            Thread.Sleep(10000);

        }

    }
}
