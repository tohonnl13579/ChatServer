using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("### Simple Chat Server ###");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.OpenTimeout = new TimeSpan(0, 0, 5);
            tcp.CloseTimeout = new TimeSpan(0, 0, 5);
            tcp.ReceiveTimeout = new TimeSpan(0, 0, 10);
            tcp.SendTimeout = new TimeSpan(0, 0, 30);
            host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            Console.WriteLine(" ChatServer is now online! ");
            Console.ReadLine();
            Console.WriteLine(" Press enter again to close the ChatServer... ");
            Console.ReadLine();
            Console.WriteLine(" Press enter one more time to close server...");
            Console.ReadLine();
            host.Close();
        }
    }
}
