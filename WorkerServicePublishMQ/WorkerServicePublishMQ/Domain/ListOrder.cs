using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WorkerServicePublishMQ.Domain 

{
    internal class ListOrder
    {
        private List<Order> _listOrder;
        public ListOrder()
        {

            _listOrder = new List<Order>(); 

        }

        public List<Order> GetOrders(int CountOrder )
        {
            for (int i = 1; i <= CountOrder; i++)
            {
                var order = new Order();
                order.OrderID = i;
                order.endereco = "Rua numero " + i.ToString();
                order.Nome = "Cliente " + i.ToString();
                order.valorPedido = i;

                _listOrder.Add(order);
            }


            return _listOrder;
        }

    }
}