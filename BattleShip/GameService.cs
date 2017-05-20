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
        Cell curShoot;

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
            //curShoot = new Cell(2,2);

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
                // самая первая
                // string dots = "2,1;2,2;2,3;2,4;6,1;6,2;6,3;9,2;9,3;3,6;4,6;5,6;8,5;9,5;7,7;4,8;1,9;1,10;6,10;9,9";

                // лучшая 1
                string dots = "1,1;1,2;1,4;1,5;1,7;1,8;1,9;1,10;3,1;3,2;3,4;3,5;3,6;3,8;3,9;3,10;8,1;7,3;8,8;6,6";

                // лучшая 2
                // string dots = "1,1;1,2;1,4;1,5;1,7;1,8;1,9;1,10;3,1;3,2;3,4;3,5;3,6;3,8;3,9;3,10;8,1;7,3;8,8;6,6";
                channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes(dots));
                Console.WriteLine($"Координаты отосланы");
            }

            if (Encoding.UTF8.GetString(e.Body) == "fire!")
            {
                curShoot = battle.Shoot();
                string curShootString;

                if (curShoot == null)
                {
                    curShoot = new Cell(2,2);
                    curShoot.X = 1;
                    curShoot.Y = 1;
                }

                curShootString = $"{ (curShoot.X + 1) },{ (curShoot.Y + 1) }";
                channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes(curShootString));
            }

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body).Contains("fire result: MISS"))
            {
                battle.AnalizaAns("MISS", curShoot);
            }

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body).Contains("fire result: HIT"))
            {
                battle.AnalizaAns("HIT", curShoot);
            }

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body).Contains("fire result: KILL"))
            {
                battle.AnalizaAns("KILL", curShoot);
            }
        }
    }
}
