using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace BattleShip
{
    class Program
    {
        void Main(string[] args)
        {
            // Установить соединение
            var connFactory = new ConnectionFactory { Uri = "amqp://group4:z3bPjU" };
            var connection = connFactory.CreateConnection();
            var channel = connection.CreateModel();

            // Настройка отправки сообщения
            var outQueue = "group4";
            channel.QueueDeclare(outQueue, exclusive: false);

            // Отправить сообщение
            byte[] body = new byte[1] { 255 };
            channel.BasicPublish(outQueue, outQueue, null, body);

            // Слушатель входящих сообщений
            var consumer = new EventingBasicConsumer(;
            var incQueue = "to_group4";
            channel.QueueDeclare(incQueue, exclusive: false);
            channel.QueueBind(incQueue, incQueue, incQueue);
            channel.BasicConsume(incQueue, true, consumer);

            consumer.Received += ProcessIncomingMessage;
        }

        void ProcessIncomingMessage(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            Console.Write("Бот подключился");
        }
    }
}
