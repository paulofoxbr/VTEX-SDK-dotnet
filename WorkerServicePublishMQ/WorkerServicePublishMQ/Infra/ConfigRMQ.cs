using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServicePublishMQ.Infra
{
    public class ConfigRMQ
    {
        public string HostName { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
        public ConfigRMQ()
        {
            HostName = "localhost";
            Queue = "Pedidos";
            Exchange = "Ecommerce-Controplan";
        }


    }
}
