using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceConsumerMQ.Infra
{
    public class ConfigConsumer
    {
        public string HostName { get; set; }
        public string Queue { get; set; }
        public string Exchange { get; }

        public ConfigConsumer()
        {
 
            HostName = "localhost";
            Queue = "Pedidos";
            Exchange = "Controplan";

        }


    }
}
