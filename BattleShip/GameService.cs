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

                //Обычные
              //  dots[0] = "2,1; 2,2; 2,3; 2,4; 6,1; 6,2; 6,3; 9,2; 9,3; 3,6; 4,6; 5,6; 8,5; 9,5; 7,7; 4,8; 1,9; 1,10; 6,10; 9,9";
              //  dots[1] = "2,1; 2,2; 2,3; 2,4; 6,1; 6,2; 6,3; 9,2; 9,3; 3,6; 4,6; 5,6; 8,5; 9,5; 7,7; 4,8; 1,9; 1,10; 6,10; 9,9";
               // dots[2] = "2,1; 2,2; 2,3; 2,4; 6,1; 6,2; 6,3; 9,2; 9,3; 3,6; 4,6; 5,6; 8,5; 9,5; 7,7; 4,8; 1,9; 1,10; 6,10; 9,9";

               
                Random rnd = new Random();

                int count = rnd.Next(0, 3);      
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
            //if (Encoding.UTF8.GetString(e.Body).Contains("winner"))
            //{
            //    _endEvent.Set();
            //}
        }
    }
}
