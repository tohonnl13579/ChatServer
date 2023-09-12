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
            tcp.MaxBufferPoolSize = 10000;
            tcp.MaxReceivedMessageSize = 500000;
            tcp.MaxBufferSize = 500000;
            tcp.ReaderQuotas.MaxArrayLength = 10000;
            tcp.ReaderQuotas.MaxArrayLength = 10000;
            tcp.ReaderQuotas.MaxDepth = 10;
            tcp.ReaderQuotas.MaxBytesPerRead = 10000;
            tcp.ReaderQuotas.MaxStringContentLength = 10000;

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
