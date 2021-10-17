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
        public string Vhost { get; set; }

        public ConfigRabbitMQ()
        {
            ServerRabbitMQurl = "172.17.0.6";
            ExchangeName = "Vtex";
            QueueName = "Pedidos";
            UserMQ = "guest";
            PassWord = "guest";
            Port = 5672;
            Vhost = "vHostTet";

        }
    }
}
