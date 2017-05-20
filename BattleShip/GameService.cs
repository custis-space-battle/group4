using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BattleShip
{
    public class GameService
    {
        private string outQueue;
        private string incQueue;
        private IModel channel;
        LetsShoot battle;

        public void OnStart()
        {
            // Установить соединение
            Console.WriteLine("Выберете сервер");
            string serverName = Console.ReadLine();
            var connFactory = new ConnectionFactory { Uri = $"amqp://group4:z3bPjU@91.241.45.69/{serverName}" };
            var connection = connFactory.CreateConnection();
            channel = connection.CreateModel();
            Console.WriteLine($"Бот подключился к серверу { serverName }\n");

            // Настройка отправки сообщения
            outQueue = "group4";
            channel.QueueDeclare(outQueue, exclusive: false);

            // Слушатель входящих сообщений
            var consumer = new EventingBasicConsumer(channel);
            incQueue = "to_group4";
            channel.QueueDeclare(incQueue, exclusive: false);
            channel.QueueBind(incQueue, incQueue, incQueue);
            channel.BasicConsume(incQueue, true, consumer);

            GetMode();

            battle = new  LetsShoot();

            consumer.Received += ProcessIncomingMessage;
        }

        void GetMode()
        {
            Console.WriteLine("Выберете мод");
            string modeName = Console.ReadLine();
            channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes($"start: { modeName }"));
            Console.WriteLine($"Выбран мод { modeName }\n");
        }

        void ProcessIncomingMessage(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body) == "prepare!")
            {
                string dots = "2,1;2,2;2,3;2,4;6,1;6,2;6,3;9,2;9,3;3,6;4,6;5,6;8,5;9,5;7,7;4,8;1,9;1,10;6,10;9,9";
                channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes(dots));
                Console.WriteLine($"Координаты отосланы");
            }

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body) == "fire!")
            {
                Cell curShoot = new Cell();         
                curShoot = battle.Shoot();
                Console.WriteLine(curShoot.X);
                Console.WriteLine(curShoot.Y);

                string curShootString = $"{ curShoot.X },{ curShoot.Y }";
                channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes(curShootString));
            }

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body) == "MISS")
            {
             
            }

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body) == "MISSAGAIN")
            {
               
            }
        }
    }
}
