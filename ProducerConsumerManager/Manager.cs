using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ProducerConsumerManager
{
    class Manager
    {
        public List<AbstractProcessHost> Processes { get; private set; }
        public Configuration BPCConfiguration { get; private set; }
        public Logger BPCLogger { get; private set; }
        public Manager()
        {
            BPCLogger = new Logger(true);
            BPCConfiguration = Configuration.CreateConfiguration();
            Processes = new List<AbstractProcessHost>();
        }

        public void StartAndWait()
        {
            //Create Consumers
            for (var i = 0; i < BPCConfiguration.ConsumerConfiguration.NumberOfProcesses; i++)
            {
                var consumer = new ConsumerHost();
                if (!consumer.Start()) continue;
                Processes.Add(consumer);
            }
            //Create Producers
            for (var i = 0; i < BPCConfiguration.ProducerConfiguration.NumberOfProcesses; i++)
            {
                var producer = new ProducerHost();
                if (producer.Start()) continue;
                Processes.Add(producer);
            }

            //Wait for all of them to exit
            foreach (var process in Processes)
            {
                process.Close();
            }
            BPCLogger.Log("Finished all process");

        }
    }
}
