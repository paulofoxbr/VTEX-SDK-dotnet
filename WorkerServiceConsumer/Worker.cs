using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WorkerServiceConsumerMQ.Domain;
using WorkerServiceConsumerMQ.Infra;
using WorkerServiceConsumerMQ.Service;

namespace WorkerServiceConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IModel _channel { get; set; }

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var configConsumer = new ConfigConsumer();
            var factory = new ConnectionFactory()
            {
                HostName = configConsumer.HostName,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                AutomaticRecoveryEnabled = true
            };
            using (var connection = factory.CreateConnection())
            using ( _channel = connection.CreateModel())
            {

                _channel.BasicQos(0, 1, false);
                _channel.QueueDeclare(queue: configConsumer.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);


                _channel.QueueBind(queue: configConsumer.Queue,
                    exchange: configConsumer.Exchange,
                    routingKey: string.Empty,
                    arguments: null);


                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Consumer_Received;

                _channel.BasicConsume(queue: configConsumer.Queue,
                    autoAck: false,
                    consumer: consumer);
            }


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at : {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);
            }
        }

        private void  Consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            OrderConsumer messageJson;
            try
            {
                _logger.LogInformation("Consumer_Received at : {time}", DateTimeOffset.Now);

                var message = Encoding.UTF8.GetString(body);
                messageJson = JsonSerializer.Deserialize<OrderConsumer>(message);
                Console.WriteLine($"Oder : {messageJson.Nome} - {messageJson.endereco}");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (_channel.IsOpen)
                {
                    _channel.BasicReject(ea.DeliveryTag, true);
                }
                throw;
            }

            try
            {
                // doanything
                if (_channel.IsOpen) { _channel.BasicAck(ea.DeliveryTag, false); }
                else { _logger.LogInformation("Channel is closed at : {time}", DateTimeOffset.Now); }

            }
            catch (Exception e)
            {
                if (_channel.IsOpen)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }

                throw;
            }


        }
    }
}
