using System.Diagnostics;
using Utility;

namespace ProducerConsumerManager
{
    abstract class AbstractProcessHost
    {
        public Logger ProcessLogger { get; set; }
        protected AbstractProcessHost()
        {
            ProcessLogger = new Logger(true);
        }
        public Process ProcessHost { get; private set; }
        public abstract string Name { get; }
        public bool Start()
        {
            ProcessHost = Process.Start(Name + ".exe");
            if (ProcessHost != null)
            {
                ProcessLogger.Log("Created " + Name + " process with process id" + ProcessHost.Id);
                return true;
            }
            ProcessLogger.Log("Failed to Create process " + Name);
            return false;
        }

        public void Close()
        {
            ProcessHost.WaitForExit();
            ProcessLogger.Log(Name + " with process id " + ProcessHost.Id + " closed");
            ProcessHost.Close();
        }
    }

    class ConsumerHost : AbstractProcessHost
    {
        public override string Name
        {
            get { return "Consumer"; }
        }
    }

    class ProducerHost : AbstractProcessHost
    {
        public override string Name
        {
            get { return "Producer"; }
        }
    }
}
