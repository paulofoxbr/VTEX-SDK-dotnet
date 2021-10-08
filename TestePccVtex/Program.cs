using CrispyWaffle.Composition;
using CrispyWaffle.Log;
using CrispyWaffle.Log.Adapters;
using CrispyWaffle.Log.Handlers;
using CrispyWaffle.Log.Providers;
using System;
using System.Collections.Generic;
using VTEX;
using VTEX.Transport;

namespace TestePccVtex
{
    class Program
    {
        static void Main(string[] args)
        {
            // eCommerceContext();
            //TestaLogger();


        }

        private static void TestaLogger()
        {

            //Registering the standard console log adapter to be used by console log provider. 
            ServiceLocator.Register<IConsoleLogAdapter, StandardConsoleLogAdapter>(LifeStyle.SINGLETON);

            //Registering the null exception handler for the method LogConsumer.Handle, this means that no action will be executed for exceptions handled by LogConsumer.
            ServiceLocator.Register<IExceptionHandler, NullExceptionHandler>(LifeStyle.SINGLETON);

            //Adding console provider to LogConsumer, the log provider will use the registered IConsoleLogAdapter.
            //LogConsumer.AddProvider<ConsoleLogProvider>();
            LogConsumer.AddProvider<TextFileLogProvider>();
            

            LogConsumer.Info("Hello world Crispy Waffle");

            LogConsumer.Debug("Current time: {0:hh:mm:ss}", DateTime.Now);

            LogConsumer.Warning("Press any key to close the program!");

            Console.ReadKey();

        }

        private static void eCommerceContext()
        {
            const string lojaEcommerce = "epocacosmeticos";
            const string AppKey = "vtexappkey-epocacosmeticos-SCKHKR";
            const string AppToken = "YNYGHPDSGGBQOHVMSEFWXUSTAITVOZMXVQILNFESMAASWZRVKXSLHTVDVFGYWIDHUEUYYJYMAMKWTZWBRYKRHAPCUEQIZFWEXTYRETWAYFLUJYJVIFHYXLATOSDTTQNV";
            //var codigoPedidoVtex = "v40379447epcc-001";  // Pedido não existente.
            var codigoPedidoVtex = "v40379447epcc-01"; // Pedido Existente
            // var codigoPedidoVtex = "BWW-Lojas_Americanas-285264647201";
            try
            {
                var vtex = new VTEXContext(lojaEcommerce, AppKey, AppToken);
                //var order = vtex.GetOrder(codigoPedidoVtex);

                var orders = vtex.GetOrdersList(VTEX.Enums.OrderStatus.READY_FOR_HANDLING);

                var ret = putOrdersOnQueue(orders);




                Console.WriteLine("Lista de Orders");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool putOrdersOnQueue(IEnumerable<List> orders)
        {

            foreach (var item in orders)
            {
                var marketPlaceOrderID = item.MarketPlaceOrderId;
                var orderID = item.OrderId;
                var sequence = item.Sequence;
                var authorizedDate = item.AuthorizedDate;
                var str = item.ToString();
                Console.WriteLine(orderID +" -- " + item.ClientName);
                // code to put order in queue on RabbitMq.

            }

            return true;

        }
    }
}
