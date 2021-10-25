using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerServiceConsumerMQ.Domain;

namespace WorkerServiceConsumerMQ
{

    public class Worker1 : BackgroundService
    {
        private readonly ILogger<Worker1> _logger;

        public Worker1(ILogger<Worker1> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                //Port = 5672,
                //UserName = "user",
                //Password = "password",
            };

            using var connection = connectionFactory.CreateConnection();

            using var rabbitMQmodel = connection.CreateModel();

            rabbitMQmodel.QueueDeclare("Pedidos", true, false, false, null);

            rabbitMQmodel.QueueBind(queue: "Pedidos",
                                    exchange: "Controplan",
                                    routingKey: string.Empty,
                                    arguments: null);


            rabbitMQmodel.BasicQos(0, 100, true);

            var consumer = new EventingBasicConsumer(rabbitMQmodel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                        //Console.WriteLine(message);
                        Console.WriteLine("Mensagem consumida.");

                    /*##########*/
                    //Business Code
                    var messageJson = JsonSerializer.Deserialize<OrderConsumer>(message);
                    Console.WriteLine($"Oder : {messageJson.Nome} - {messageJson.endereco}");
                    /*##########*/
                    rabbitMQmodel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    rabbitMQmodel.BasicNack(ea.DeliveryTag, false, true);
                    throw;
                }

            };
            rabbitMQmodel.BasicConsume(queue: "Pedidos",
                                 autoAck: false,
                                 consumer: consumer);



            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }


}
