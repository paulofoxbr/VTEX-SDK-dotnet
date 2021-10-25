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
        private IConnection _connection { get; set; }
        private IModel _channel { get; set; }

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

                _channel = channel;
                _connection = connection;

                channel.BasicQos(0, 10, false);
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
                //consumer.Shutdown += Consumer_Shutdown;
                consumer.Received += Consumer_Received;

                channel.BasicConsume(queue: configConsumer.Queue,
                    autoAck: false,
                    consumer: consumer);
            }
            


        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs ea)
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
                if (_channel.IsOpen)
                {
                    _channel.BasicReject(ea.DeliveryTag, true);
                }
                throw;
            }



            try
            {
                // doanything
                if (_channel.IsOpen)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
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

        private void Consumer_Shutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("shutdown event dispardo");
            throw new NotImplementedException();
        }
    }
}
