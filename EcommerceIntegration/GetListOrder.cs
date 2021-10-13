using System;
using System.Collections.Generic;
using VTEX;
using VTEX.Transport;

namespace EcommerceIntegration
{
    public class EcommerceOrders
    {
        const string lojaEcommerce = "epocacosmeticos";
        const string AppKey = "vtexappkey-epocacosmeticos-SCKHKR";
        const string AppToken = "YNYGHPDSGGBQOHVMSEFWXUSTAITVOZMXVQILNFESMAASWZRVKXSLHTVDVFGYWIDHUEUYYJYMAMKWTZWBRYKRHAPCUEQIZFWEXTYRETWAYFLUJYJVIFHYXLATOSDTTQNV";
        //var codigoPedidoVtex = "v40379447epcc-001";  // Pedido não existente.
        // var codigoPedidoVtex = "v40379447epcc-01"; // Pedido Existente
        // var codigoPedidoVtex = "BWW-Lojas_Americanas-285264647201";
        public VTEX.VTEXContext VTEXContext   { get; set; }
        public  EcommerceOrders()
        {
            VTEXContext = new VTEXContext(lojaEcommerce, AppKey, AppToken);
        }

        public List<Pedidos> RetornaListaDePedidos()
        {


            var ordersList = VTEXContext.GetOrdersList(VTEX.Enums.OrderStatus.READY_FOR_HANDLING);

            var  ListaDePedidos = new EcommerceListOrders();
            foreach (var item in ordersList)
            {
                ListaDePedidos.AddOrder(item.OrderId, item.Sequence, item.AuthorizedDate, item.ShippingEstimatedDateMax);
            }


            return ListaDePedidos.ListaDePedidos;
        }

        public object GetOrderDetail(string pedido)
        {

            var orderDetail = VTEXContext.GetOrder(pedido);

            return orderDetail;
            // throw new NotImplementedException();
        }
    }

    public class EcommerceListOrders
    {
        public  List<Pedidos> ListaDePedidos;
        public EcommerceListOrders()
        {
            ListaDePedidos = new List<Pedidos>();
        }
        internal void AddOrder(string orderId, int sequence, DateTime? authorizedDate, DateTime? shippingEstimatedDateMax)
        {
            ListaDePedidos.Add( new Pedidos(orderId,sequence,authorizedDate,shippingEstimatedDateMax));
        }
    }

    public class Pedidos
    {
        public string Pedido { get; set; }
        public string idPedido { get; set; }
        public DateTime? DataAurorizacao { get; set; }
        public DateTime? DataMaxEntrega { get; set; }
        public Pedidos(string orderId, int sequence, DateTime? authorizedDate, DateTime? shippingEstimatedDateMax)
        {
            Pedido = orderId;
            idPedido = sequence.ToString();
            DataAurorizacao = authorizedDate;
            DataMaxEntrega = shippingEstimatedDateMax;
        }


    }
}
