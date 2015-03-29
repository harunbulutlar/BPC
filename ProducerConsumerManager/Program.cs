using System.Collections.Generic;
using System.Threading;
using Utility;

namespace ProducerConsumerManager
{
    class Program
    {
        static void Main()
        {
            var manager = new Manager();
            manager.StartAndWait();


        }

    }
}
