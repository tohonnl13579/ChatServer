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
        private static int portNum = 8100;
        static void Main(string[] args)
        {
            Console.WriteLine("### Simple Chat Server ###");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            string url = "net.tcp://0.0.0.0:8100/DataService";

            tcp.OpenTimeout = new TimeSpan(0, 0, 5);
            tcp.CloseTimeout = new TimeSpan(0, 0, 5);
            tcp.ReceiveTimeout = new TimeSpan(0, 0, 10);
            tcp.SendTimeout = new TimeSpan(0, 0, 30);
            tcp.MaxBufferPoolSize = 50000000; //50MB
            tcp.MaxReceivedMessageSize = 50000000; //50MB
            tcp.MaxBufferSize = 50000000; //50MB
            tcp.ReaderQuotas.MaxArrayLength = 100000;
            tcp.ReaderQuotas.MaxDepth = 10;
            tcp.ReaderQuotas.MaxBytesPerRead = 10000000; //10MB 
            tcp.ReaderQuotas.MaxStringContentLength = 1000000; //1MB

            //Creates port for connection
            host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, urlBuilder());
            host.Open();

            Console.WriteLine(" ChatServer is now online! ");
            Console.ReadLine();
            Console.WriteLine(" Press enter again to close the ChatServer... ");
            Console.ReadLine();
            Console.WriteLine(" Press enter one more time to close server...");
            Console.ReadLine();
            host.Close();
        }

        private static string urlBuilder()
        {
            string fullUrl, baseUrlFront = "net.tcp://localhost:", baseUrlTail = "/DataService";
            fullUrl = (baseUrlFront + portNum + baseUrlTail);
            return fullUrl;
        }
    }
}
