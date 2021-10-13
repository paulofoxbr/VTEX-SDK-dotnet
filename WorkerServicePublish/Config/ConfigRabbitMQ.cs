using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServicePublish.Config
{
    class ConfigRabbitMQ
    {
        public string ServerRabbitMQurl { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string UserMQ { get; set; }
        public string PassWord { get; set; }
        public int Port { get; internal set; }

        public ConfigRabbitMQ()
        {
            ServerRabbitMQurl = "localhost";
            ExchangeName = "Vtex";
            QueueName = "Pedidos";
            UserMQ = "CtpAgente";
            PassWord = "Ctp2101";
            Port = 5672;

        }
    }
}
