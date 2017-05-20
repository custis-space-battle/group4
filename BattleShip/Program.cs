using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BattleShip
{
    class Program
    {
        static void Main(string[] args)
        {
            // Установить соединение
            var connFactory = new ConnectionFactory { Uri = "amqp://group4:z3bPjU@91.241.45.69/debug" };
            var connection = connFactory.CreateConnection();
            var channel = connection.CreateModel();
            Console.WriteLine("Бот подключился к серверу");

            // Настройка отправки сообщения
            var outQueue = "group4";
            channel.QueueDeclare(outQueue, exclusive: false);

            // Слушатель входящих сообщений
            var consumer = new EventingBasicConsumer(channel);
            var incQueue = "to_group4";
            channel.QueueDeclare(incQueue, exclusive: false);
            channel.QueueBind(incQueue, incQueue, incQueue);
            channel.BasicConsume(incQueue, true, consumer);

            consumer.Received += ProcessIncomingMessage;

            // Отправить сообщение
            channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes("start: bot"));
            Console.WriteLine("Сообщение отправлено");
        }

        static void ProcessIncomingMessage(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body)); 
        }
    }
}
