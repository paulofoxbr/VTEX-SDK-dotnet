using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerServicePublishMQ.Infra;

namespace WorkerServicePublishMQ.Service
{
    public class PublishMessage
    {
        public PublishMessage() { }
        public void Publish(string Message)
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

                    channel.QueueDeclare(queue: Config.Queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    
                    var bodyMessage = Encoding.UTF8.GetBytes(Message);

                    channel.BasicPublish(exchange: "",
                        routingKey: Config.Queue,
                        basicProperties: null,
                        mandatory: true);

                    channel.WaitForConfirms(new TimeSpan(0, 0, 5));

                }
                catch (Exception)
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
