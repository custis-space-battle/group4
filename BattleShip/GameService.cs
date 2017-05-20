using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace BattleShip
{
    public class GameService
    {
        private string outQueue;
        private string incQueue;
        private IModel channel;
        LetsShoot battle;
        Cell curShoot;

        private ManualResetEvent _endEvent = new ManualResetEvent(false);

        public void OnStart(string serverName, string modeName)
        {
            // Установить соединение

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

            channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes($"start: { modeName }"));

            battle = new  LetsShoot();
            //curShoot = new Cell(2,2);

            consumer.Received += ProcessIncomingMessage;
            _endEvent.WaitOne();

            connection.Dispose();
        }



        void ProcessIncomingMessage(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Body));

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body) == "prepare!")
            {
                string[] dots = new string[3];

                dots[0] = "2,6; 2,8; 2,9; 3,1; 3,2; 3,3; 3,4; 4,6; 5,6; 6,6; 6,9; 6,1; 6,2; 6,3; 10,1; 9,3; 9,4; 9,6; 9,7; 9,9"; // самый первый
                dots[1] = "2,1; 2,2; 2,4; 2,5; 2,7; 2,8; 2,9; 2,10; 9,1; 9,2; 9,3; 9,5; 9,6; 9,8; 9,9; 9,10; 3,4; 6,5; 8,8; 4,8";// две вертикальные со смешением 
                dots[2] = "1,1; 1,2; 1,3; 1,4; 1,6; 1,7; 1,8; 1,9; 1,10; 10,2; 10,3; 10,5; 10,6; 10,7; 10,9; 10,10; 5,3; 7,4; 2,2; 5,9"; // две вертикальные по краям
                //dots[2] = 
              
                Random rnd = new Random();

                int count = rnd.Next(0, 3);
                // самая первая
              //   string dots = "2,1;2,2;2,3;2,4;6,1;6,2;6,3;9,2;9,3;3,6;4,6;5,6;8,5;9,5;7,7;4,8;1,9;1,10;6,10;9,9";

                // для компа
               // string dots = "2,1;2,2;2,4;2,5;2,7;2,8;2,9;2,10;9,1;9,2;9,3;9,5;9,6;9,8;9,9;9,10;3,4;6,5;8,8;4,8";

                // для игроков
               // string dots = "1,1; 1,2; 1,4; 1,5; 1,7; 1,8; 1,9; 1,10; 10,1; 10,2; 10,3; 10,5; 10,6; 10,8; 10,9; 10,10; 6,4; 8,5; 3,3; 5,7";
                channel.BasicPublish(outQueue, outQueue, null, Encoding.UTF8.GetBytes(dots[count]));
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

            // Передать координаты своих кораблей
            if (Encoding.UTF8.GetString(e.Body).Contains("winner"))
            {
                _endEvent.Set();
            }
        }
    }
}
