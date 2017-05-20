using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BattleShip
{
    public class Program
    {
        static string GetMode()
        {
            Console.WriteLine("Выберете мод");
            string modeName = Console.ReadLine();
            Console.WriteLine($"Выбран мод { modeName }\n");

            return modeName;
        }

        static void Main()
        {
            Console.WriteLine("Выберете сервер");
            string serverName = Console.ReadLine();
            var modeName = GetMode();


            while (true)
            {
                var service = new GameService();
                service.OnStart(serverName, modeName);
            }
        }
    }
}
