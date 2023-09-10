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
    /*This is the window that contains the room selection and chat rooms*/
    public partial class ChatRoomWindow : Window
    {
        private DataServerInterface foob;
        private ChannelFactory<DataServerInterface> foobFactory;
        private string loggedUser, currChatRoom;
        private int maxConnectAtt;
        public ChatRoomWindow(string user)
        {
            InitializeComponent();
            loggedUser = user;
            maxConnectAtt = 6;
            ChatRoomWarning_Label.Content = "";
            connectToServer();
            updateRooms();
        }

        private bool connectToServer()
        {
            bool connect = true;
            NetTcpBinding tcpB = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
            Label_LoggedAs.Content = "Logged as: " + loggedUser;

            //Once it reaches maximum connection attempts, client fails to connect to server.
            if(maxConnectAtt <= 0)
            {
                connect = false;
            }
            else
            {
                maxConnectAtt--;
            }

            return connect;
        }

        //Event Handling


        //Updating GUI Lists
        private void updateRooms()
        {
            ChatRoomWarning_Label.Content = "";
            ListBox_RoomList.Items.Clear();
            try
            {
                List<string> roomList = foob.GetChatRoomList();
                for(int i = 0; i < roomList.Count; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = roomList[i];
                    ListBox_RoomList.Items.Add(item);
                }
            }
            catch (CommunicationException cE)
            {
                ChatRoomWarning_Label.Content = "Connection Lost!: " + cE.Message;
                connectToServer();
            }
            catch (Exception eR)
            {
                ChatRoomWarning_Label.Content = "Exception occured: " + eR.Message;
            }

        }

        private void updateMessages()
        {
            ChatRoomWarning_Label.Content = "";
        }

        private void updateUsers()
        {
            ChatRoomWarning_Label.Content = "";
            try
            {
                HashSet<string> userOnline = foob.GetUserOnline(currChatRoom);
                ListBox_UserList.Items.Clear();
                if (userOnline.Count > 0)
                {
                    foreach(string user in userOnline)
                    {
                        ListBoxItem item = new ListBoxItem();
                        item.Content = user;
                        ListBox_UserList.Items.Add(user);
                    }
                }
            }
            catch (CommunicationException cE)
            {
                ChatRoomWarning_Label.Content = "Connection Lost!: " + cE.Message;
                connectToServer();
            }
            catch (Exception eR)
            {
                ChatRoomWarning_Label.Content = "Exception occured: " + eR.Message;
            }
        }
    }
}
