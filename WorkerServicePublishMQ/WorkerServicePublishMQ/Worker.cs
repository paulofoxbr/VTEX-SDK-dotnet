using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkerServicePublishMQ.Domain;
using WorkerServicePublishMQ.Service;
using System.Text.Json;

namespace WorkerServicePublishMQ
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int _count;
        private DateTime _dateStard;
        private DateTime _dateFinish;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _count = 0;
            _dateStard = DateTime.Now;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                MyFirstPublish();
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void MyFirstPublish()
        {
            if (_count > 10)
            {
                _dateFinish = DateTime.Now;
                _logger.LogInformation("Worker Started running at: {time}", _dateStard);
                _logger.LogInformation("Worker Fineshed running at: {time}", _dateFinish);

                return;
            }

            _count++;
            _logger.LogInformation("Worker MyFirstPublish running at: {time}", DateTimeOffset.Now);

            var MyListOrder = new ListOrder().GetOrders(100);
            var publishMessage = new PublishMessage();
            foreach (var item in MyListOrder)
            {
                try
                {
                 _logger.LogInformation("Worker count : {orderid}", item.OrderID);


                publishMessage.Publish(item);

                }
                catch (Exception e)
                {
                    _logger.LogInformation("Message error on : {orderid}", item.OrderID);
                    _logger.LogInformation("Message error on : {e}", e.Message);
                }

            }


        }

    }
}
