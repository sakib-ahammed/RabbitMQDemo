namespace GHRabbitMQDemo
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;
        private IRabbitManager _manager;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IRabbitManager manager)
        {
            _manager = manager;
            this._logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedService>();
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            // create connection
            _connection = factory.CreateConnection();

            // create channel
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare("demo.exchange.header", ExchangeType.Headers, true);
            //_channel.QueueDeclare("demo.queue.log", false, false, false, null);
            //_channel.QueueBind("demo.queue.log", "demo.exchange.fanout", "demo.queue.*", null);

            //// dynamic queue for Fanout exchange
            //this._queueName = _channel.QueueDeclare().QueueName;
            //_channel.QueueBind(this._queueName, "demo.exchange.fanout", "demo.queue.*", null);
            /// Header exchange
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("format", "pdf");
            dictionary.Add("machine", this._manager.GetRabbitMQMachineID());
            dictionary.Add("x-match", "any");

            this._queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(this._queueName, "demo.exchange.header", "", dictionary);

            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                /*if (_manager.GetRabbitMQMachineID() != ea.BasicProperties.Headers["machine"].ToString())
                {
                    Console.WriteLine($"GetRabbitMQMachineID: {_manager.GetRabbitMQMachineID()} - {ea.BasicProperties.Headers["machine"].ToString()}");
                    // received message
                    var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                    // handle the received message
                    HandleMessage(content);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }*/
                // received message
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(this._queueName, false, consumer);
            //_channel.BasicConsume("demo.queue.log", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            /*JObject message = JObject.Parse(content);
            //var message = JsonConvert.SerializeObject(content);
            if (message["machineID"].ToString() != _manager.GetRabbitMQMachineID())
            {
                // we just print this message 
                _logger.LogInformation($"consumer received message : {content}");
            }
            else
            {
                // we just print this message 
                _logger.LogInformation($"Message publish from this machine. Id- {message["machineID"].ToString()} - {_manager.GetRabbitMQMachineID()}");
            }*/

            // we just print this message 
            _logger.LogInformation($"consumer received message : {content}");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"connection shut down {e.ReplyText}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer cancelled {e.ConsumerTags}");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer unregistered {e.ConsumerTags}");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer registered {e.ConsumerTags}");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"consumer shutdown {e.ReplyText}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
