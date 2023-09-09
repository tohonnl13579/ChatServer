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


namespace ClientWPF
{
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;
        private ChannelFactory<DataServerInterface> foobFactory;
        private string loggedUser;
        public MainWindow()
        {
            InitializeComponent();

            Warning_Label.Content = "";
            Username_TextBox.Text = "";
            LoggedIn_Label.Content = "Logged in as: ";
            Button_EnterChatRoom.Visibility = Visibility.Hidden;
            Button_EnterChatRoom.IsEnabled = false; 

            connectToServer();
        }

        private void connectToServer()
        {
            NetTcpBinding tcpB = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //Button Functionality
            Warning_Label.Content = "";
            string username = Username_TextBox.Text;
            if (username.Length == 0)
            {
                Warning_Label.Content = "Username field should not be empty!";
            }
            else
            {
                bool logged = foob.addUser(username);
                if (logged)
                {
                    Warning_Label.Content = "Logged in successfully!";
                    loggedUser = username;
                    LoggedIn_Label.Content = "Logged in as: " + loggedUser;
                    Button_EnterChatRoom.Visibility= Visibility.Visible;
                    Button_EnterChatRoom.IsEnabled = true;
                }
                else
                {
                    Warning_Label.Content = "User exists!";
                }
            }
        }

        private void enterChatRoom()
        {
            ChatRoomWindow chatRoomWindow = new ChatRoomWindow(loggedUser);
            if (chatRoomWindow != null) { 
                chatRoomWindow.Show();
                this.Close();
            }
            else
            {
                Warning_Label.Content = "Failed to switch windows";
            }
        }

        private void EnterChatRoom_Click(object sender, RoutedEventArgs e)
        {
            //Button Func
            //Attempt to close the connection, so the other window can connect
            try
            {
                ((ICommunicationObject)foob).Close();
                foobFactory.Close();
                //Enter chatroom window
                enterChatRoom();
            }
            catch (Exception eR)
            {
                Console.WriteLine(eR.Message);
                Warning_Label.Content = eR.Message;
                connectToServer();
            }
        }
    }
}
