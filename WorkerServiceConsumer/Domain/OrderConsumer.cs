using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceConsumerMQ.Domain
{
    public class OrderConsumer
    {

        public int OrderID { get; set; }
        public string Nome { get; set; }
        public string endereco { get; set; }
        public decimal valorPedido { get; set; }


    }
}
