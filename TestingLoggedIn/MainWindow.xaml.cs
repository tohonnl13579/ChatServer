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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestingLoggedIn
{
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;
        private DataServerInterface foob2;
        private ChannelFactory<DataServerInterface> foobFactory;
        private ChannelFactory<DataServerInterface> foobFactory2;
        private string username = "tohonnl";
        private string chatRoom = null;
        public MainWindow()
        {
            NetTcpBinding tcpB = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
            InitializeComponent();
            UpdateRooms();
        }

        private void RoomList_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem) RoomList_ListBox.SelectedItem;
            if (chatRoom != null)
            {
                foob.LeaveChatRoom(chatRoom, username);
            }
            if (item != null)
            {
                chatRoom = foob.JoinChatRoom(item.Content.ToString(), username);
                ChatRoom_Label.Content = chatRoom;
            }
            UpdateMessages();
            UpdateUsers();
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            string message = EnterMessage_TexBox.Text;
            if (chatRoom != null && message != "")
            {
                foob.SendPublicMessage(chatRoom, username, message);
                UpdateMessages();
            }
        }

        private void UpdateMessages()
        {
            List<string> messages = foob.GetMessages(chatRoom, username);
            Message_ListBox.Items.Clear();
            if (chatRoom != null)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = messages[i];
                    Message_ListBox.Items.Add(item);
                }
            }
        }

        private void UpdateRooms()
        {
            List<string> roomList = foob.GetChatRoomList();
            RoomList_ListBox.Items.Clear();
            for (int i = 0; i < roomList.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = roomList[i];
                RoomList_ListBox.Items.Add(item);
            }
        }

        private void UpdateUsers()
        {
            HashSet<string> userOnline = foob.GetUserOnline(chatRoom);
            if (userOnline.Count > 0) 
            {
                UserOnline_ListBox.Items.Clear();
                foreach (string user in userOnline)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = user;
                    UserOnline_ListBox.Items.Add(item);
                }
            }
        }

        private void SendPrivate_Button_Click(object sender, RoutedEventArgs e)
        {
            string toUser = ToUser_TextBox.Text;
            string message = EnterMessage_TexBox.Text;
            if (chatRoom != null && toUser != "" && message != "")
            {
                foob.SendPrivateMessage(chatRoom, username, toUser, message);
                UpdateMessages();
            }
        }

        private void CreateRoom_Buton_Click(object sender, RoutedEventArgs e)
        {
            string intendName = CreateRoom_TextBox.Text;
            if (chatRoom != null)
            {
                foob.LeaveChatRoom(chatRoom, username);
            }
            if (intendName != "")
            {
                string newRoomName = foob.CreateChatRoom(intendName, username);
                if (newRoomName != null)
                {
                    chatRoom = newRoomName;
                    ChatRoom_Label.Content = chatRoom;
                    UpdateRooms();
                    UpdateMessages();
                    UpdateUsers();
                }
            }
        }
    }
}
