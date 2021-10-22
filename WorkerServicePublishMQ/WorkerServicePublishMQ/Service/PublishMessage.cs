using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkerServicePublishMQ.Domain;
using WorkerServicePublishMQ.Infra;

namespace WorkerServicePublishMQ.Service
{
    public class PublishMessage
    {
        public PublishMessage() { }
        public void Publish(Order Message)
        {
            var Config = new ConfigRMQ();
            var factory = new ConnectionFactory() {HostName=Config.HostName };
            using (var connection = factory.CreateConnection()) 
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.ConfirmSelect();

                    //channel.BasicAcks += Channel_BasicAcks;
                    //channel.BasicNacks += Channel_BasicNacks;
                    //channel.BasicReturn += Channel_BasicReturn;
                    channel.ExchangeDeclare(exchange: Config.Exchange,
                        type: "fanout",
                        durable: true,
                        autoDelete: false,
                        arguments: null);
                    channel.QueueDeclare(queue: Config.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    channel.QueueBind(queue: Config.Queue,
                        exchange: Config.Exchange,
                        routingKey: string.Empty,
                        arguments: null);

                    Console.WriteLine(Message.endereco);

                    var messagejson = JsonSerializer.Serialize(Message);
                    
                    var bodyMessage = Encoding.UTF8.GetBytes(messagejson);

                    channel.BasicPublish(exchange: Config.Exchange,
                        routingKey: Config.Queue,
                        basicProperties: null,
                        mandatory: true,
                        body:bodyMessage);

                    channel.WaitForConfirms(new TimeSpan(0, 0, 5));

                }
                catch (Exception ex)
                {

                    throw ;
                }
            }
        }

/*
        private void Channel_BasicReturn(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            
            throw new NotImplementedException();
        }

        private void Channel_BasicNacks(object sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Channel_BasicAcks(object sender, RabbitMQ.Client.Events.BasicAckEventArgs e)
        {
            throw new NotImplementedException();
        }
*/
    }
}
