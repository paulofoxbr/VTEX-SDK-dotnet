using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerWorkerService.Config
{
    public class Configurations
    {
        public string ServerRabbitMQurl { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }

        public Configurations()
        {
            ServerRabbitMQurl = "localhost";
            ExchangeName = "Vtex-pcc";
            QueueName = "Pedidos-pcc";
           
        }

    }
}
