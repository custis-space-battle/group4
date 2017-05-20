using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace BattleShip
{
    public class Program
    {
        static void Main()
        {
            var service = new GameService();
            service.OnStart();
        }
    }
}
