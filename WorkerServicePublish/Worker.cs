using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VTEX;
using EcommerceIntegration;
using RabbitMQ.Client;
using WorkerServicePublish.Config;
using System.Text;

namespace WorkerServicePublish
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int counterTest;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            counterTest = 0;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The service is starting...");

            stoppingToken.Register(() => _logger.LogInformation("The service is running in backgroud.."));


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("O valor do counterTest é " + counterTest.ToString());
                await Task.Delay(1000, stoppingToken);

                buscaPedidosEcommerce();
            }
            _logger.LogInformation("The service is stoped !");
        }

        public void buscaPedidosEcommerce()
        {

            try
            {
                var ecommerceOrders = new EcommerceOrders();
                var ListaDePedidos = ecommerceOrders.RetornaListaDePedidos();

                foreach (var item in ListaDePedidos)
                {
                    var ecommerOrderDetail = ecommerceOrders.GetOrderDetail(item.Pedido);

                    PublishMQ(ecommerOrderDetail);

                }

                counterTest++;



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message,ex);
            }
        }

        private void PublishMQ(object ecommerOrderDetail)
        {
            var configMQ = new ConfigRabbitMQ();
            //var factory = new ConnectionFactory() { HostName = configMQ.ServerRabbitMQurl, UserName=configMQ.UserMQ,Password=configMQ.PassWord };

            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                UserName = configMQ.UserMQ,
                Password = configMQ.PassWord,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                Port = configMQ.Port,
                AutomaticRecoveryEnabled = true

            };

            using IConnection connection = connectionFactory.CreateConnection();
            using IModel model = connection.CreateModel();

            // extremamente necessário converter a mensagem para byte
            var message = System.Text.Json.JsonSerializer.Serialize(ecommerOrderDetail);
            var messageBody = Encoding.UTF8.GetBytes(message);


            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.DeliveryMode = 2;
            basicProperties.Headers = new Dictionary<String, Object>
            {
                {"content-type","application/json" }
            };

            model.ConfirmSelect();
            model.ExchangeDeclare(exchange:configMQ.ExchangeName,type:"FANOUT",durable:true,autoDelete:false,arguments: null);
            model.QueueDeclare(queue:configMQ.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            model.QueueBind(queue:configMQ.QueueName,
                exchange:configMQ.ExchangeName,
                routingKey:string.Empty,
                arguments:null
                );
            model.BasicQos(0, 100, false);
           
            // publicação da Mensagem se dá neste método somente...
            model.BasicPublish(exchange: configMQ.ExchangeName,
                routingKey: configMQ.QueueName,
                basicProperties, 
                messageBody);
                            
            //throw new NotImplementedException();
        }
    }
}
