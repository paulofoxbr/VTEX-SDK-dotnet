﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServicePublishMQ.Domain
{
    public class Order
    {
        public int OrderID { get; set; }
        public string Nome { get; set; }
        public string endereco { get; set; }
        public decimal valorPedido { get; set; }
        public Order() 
        {
            OrderID = 0;
        }
    }
}