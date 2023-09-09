using ServerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientWPF
{
    //This is the window that contains the room selection and chat rooms

    public partial class ChatRoomWindow : Window
    {
        private DataServerInterface foob;
        private ChannelFactory<DataServerInterface> foobFactory;
        private string loggedUser;
        public ChatRoomWindow(string user)
        {
            InitializeComponent();
            loggedUser = user;
            ChatRoomWarning_Label.Content = "";
            connectToServer();
        }

        private void connectToServer()
        {
            NetTcpBinding tcpB = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
            ChatRoomWarning_Label.Content = loggedUser;
        }
    }
}
