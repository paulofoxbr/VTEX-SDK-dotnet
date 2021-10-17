using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VTEX;
using EcommerceIntegration;
using RabbitMQ;
using RabbitMQ.Client;
using WorkerServicePublish.Config;
using System.Text;
using RabbitMQ.Client.Events;

namespace WorkerServicePublish
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The service is starting...");

            stoppingToken.Register(() => _logger.LogInformation("The service is running in backgroud.."));

            
            //ConsumerMQ();
            buscaPedidosEcommerce();


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);

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
                    //var ecommerOrderDetail = ecommerceOrders.GetOrderDetail(item.Pedido);

                    var ecommerceOrderDetail = ecommerceOrders.GetOrderDetailJson(item.Pedido);
                    if (ecommerceOrderDetail != null)
                    {
                        PublishMQ(ecommerceOrderDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void PublishMQ(string ecommerOrderDetail)
        {
            try
            {


                var configMQ = new ConfigRabbitMQ();
                var factory = new ConnectionFactory()
                {
                    HostName = configMQ.ServerRabbitMQurl,
                };


                // ConnectionFactory connectionFactory = new(){ HostName = configMQ.ServerRabbitMQurl };
                //connectionFactory.ClientProvidedName = "app:audit component:event-consumer";

                using (var connection = factory.CreateConnection())
                {
                    using (var model = connection.CreateModel())
                    {
                        // extremamente necessário converter a mensagem para byte
                        var message = System.Text.Json.JsonSerializer.Serialize(ecommerOrderDetail);
                        //var message = ecommerOrderDetail;
                        var messageBody = Encoding.UTF8.GetBytes(message);


                        IBasicProperties basicProperties = model.CreateBasicProperties();
                        //basicProperties.Persistent = true;
                        basicProperties.DeliveryMode = 2;
                        basicProperties.Headers = new Dictionary<String, Object>
                        {
                            {"content-type","application/json" }
                        };

                        /*
                                                model.ConfirmSelect();
                                                model.ExchangeDeclare(exchange: configMQ.ExchangeName, type: "FANOUT", durable: true, autoDelete: false, arguments: null);
                                                model.QueueDeclare(queue: configMQ.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                                                model.QueueBind(queue: configMQ.QueueName,
                                                    exchange: configMQ.ExchangeName,
                                                    routingKey: string.Empty,
                                                    arguments: null

                                                    ); ;
                                               // model.BasicQos(0, 100, false);
                        */
                        // publicação da Mensagem se dá neste método somente...
                        model.BasicPublish(exchange: "",
                            routingKey: "pedidosPCC",
                            basicProperties: null,
                            messageBody);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            //throw new NotImplementedException();
        }
        public void ConsumerMQ()
        {
            var configMQ = new ConfigRabbitMQ();

            var factory = new ConnectionFactory()
            {
                HostName = configMQ.ServerRabbitMQurl,
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.QueueDeclare(queue: "pedidosPCC",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var bodyMessage = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(bodyMessage);
                            var order = System.Text.Json.JsonSerializer.Deserialize<VTEX.Transport.Order>(message);


                            Console.WriteLine($"Pedido:{order.OrderId} | {order.ClientProfileData.FirstName} | {order.ClientProfileData.LastName}");
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                            throw;
                        }
                    };


                    channel.BasicConsume(queue: "pedidosPCC",
                        autoAck: false,
                        consumer: consumer);

                }
            }
        }

    }
}
