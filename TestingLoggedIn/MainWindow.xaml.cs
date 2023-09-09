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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;
        private ChannelFactory<DataServerInterface> foobFactory;
        private string username = "jason";
        private string chatRoom = null;
        public MainWindow()
        {
            NetTcpBinding tcpB = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
            InitializeComponent();
            List<string> roomList = foob.GetChatRoomList();
            for (int i = 0; i< roomList.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = roomList[i];
                RoomList_ListBox.Items.Add(item);
            }
        }

        private void RoomList_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem) RoomList_ListBox.SelectedItem;
            chatRoom = item.Content.ToString();
            ChatRoom_Label.Content = chatRoom;
            UpdateMessages(chatRoom);

        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            string message = EnterMessage_TexBox.Text;
            if (chatRoom != null && message != "")
            {
                foob.SendPublicMessage(chatRoom, username, message);
                UpdateMessages(chatRoom);
            }
        }

        private void UpdateMessages(string roomName)
        {
            List<string> messages = foob.GetMessages(roomName, username);
            Message_ListBox.Items.Clear();
            for (int i = 0; i < messages.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = messages[i];
                Message_ListBox.Items.Add(item);
            }
        }

        private void SendPrivate_Button_Click(object sender, RoutedEventArgs e)
        {
            string toUser = ToUser_TextBox.Text;
            string message = EnterMessage_TexBox.Text;
            if (chatRoom != null && toUser != "" && message != "")
            {
                foob.SendPrivateMessage(chatRoom, username, toUser, message);
                UpdateMessages(chatRoom);
            }
        }
    }
}
