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

            //Set all the default state of all window elements where needed
            Warning_Label.Content = "";
            Username_TextBox.Text = "";
            LoggedIn_Label.Content = "Logged in as: ";
            Button_EnterChatRoom.Visibility = Visibility.Hidden;
            Button_EnterChatRoom.IsEnabled = false; 

            connectToServer();
        }

        //Used to reconnect to the server
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
            if (username.Equals("") || username == null)
            {
                //Case of Empty given field by user
                Warning_Label.Content = "Username field should not be empty!";
            }
            else
            {
                Warning_Label.Content = "Processing request..";
                bool logged = foob.AddUser(username);
                if (logged)
                {
                    //Enable the "Enter ChatRoom Button"
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

        //This function is called once user is logged in and they press the "Enter ChatRoom Button"
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
                //If error, catch and try to reconnect to server
                Console.WriteLine(eR.Message);
                Warning_Label.Content = eR.Message;
                connectToServer();
            }
        }
    }
}
