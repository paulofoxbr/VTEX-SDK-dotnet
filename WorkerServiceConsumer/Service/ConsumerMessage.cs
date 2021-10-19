using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //channel.BasicQos(0, 100, false);
                    channel.QueueDeclare(queue: configConsumer.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        
                        var message = Encoding.UTF8.GetString(body);

                        //channel.BasicReject(ea.DeliveryTag, true); em caso de problemas de serielização.
                        // a mensagem é incopativel para processar.

                        try
                        {
                            // do anything
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception)
                        {
                            channel.BasicNack(ea.DeliveryTag, false, true);

                            throw;
                        }
                    };

                    channel.BasicConsume(queue: configConsumer.Queue,
                        autoAck: false,
                        consumer:consumer);
                }
            }


        }
    }
}
