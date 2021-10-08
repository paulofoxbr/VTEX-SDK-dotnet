using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsumerWorkerService.Config;
using RabbitMQ.Client.Events;

namespace ConsumerWorkerService
{
    public static class ConsumerWorkerService
    {
        private  static int CountTask { get; set; }
       // private static Config Config;
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            CountTask = 0;

            var Config = new Configurations();
    
            
            var factory = new ConnectionFactory() {HostName = Config.ServerRabbitMQurl };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: Config.QueueName, durable: true, exclusive: false, arguments: null );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received +=  onReceived;

            channel.BasicConsume(queue: Config.QueueName, autoAck:false, consumer: consumer);
        }

        private static void onReceived(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            foreach (var item in body)
            {
                var x = item; // para inspesionar o item....
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
