using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerServiceConsumerMQ.Domain;
using WorkerServiceConsumerMQ.Infra;

namespace WorkerServiceConsumerMQ.Service
{
    public class ConsumerMessage
    {
        public ConsumerMessage()
        {

        }

        public void GetQueueMessage()
        {
            var configConsumer = new ConfigConsumer();
            var factory = new ConnectionFactory()
            {
                HostName = configConsumer.HostName,
                NetworkRecoveryInterval = TimeSpan.FromMinutes(10),
                AutomaticRecoveryEnabled = true
            };
            using (var connection = factory.CreateConnection())


            using (var channel = connection.CreateModel())
            {
                channel.BasicQos(0, 100, false);
                channel.QueueDeclare(queue: configConsumer.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(queue: configConsumer.Queue,
                    exchange: configConsumer.Exchange,
                    routingKey: string.Empty,
                    arguments: null);



                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    OrderConsumer messageJson;
                    try
                    {

                        var message = Encoding.UTF8.GetString(body);
                        messageJson = JsonSerializer.Deserialize<OrderConsumer>(message);
                        Console.WriteLine($"Oder : {messageJson.Nome}");


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        if (channel.IsOpen)
                        {
                            channel.BasicReject(ea.DeliveryTag, true);
                        }
                        throw;
                    }



                    try
                    {
                            // doanything
                            if (channel.IsOpen)
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception e)
                    {
                        if (channel.IsOpen)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }

                        throw;
                    }
                };

                channel.BasicConsume(queue: configConsumer.Queue,
                    autoAck: true,
                    consumer: consumer);
            }
            


        }
    }
}
