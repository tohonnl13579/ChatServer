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

namespace Client
{
    // <summary>
    // Interaction logic for MainWindow.xaml
    // </summary>
    // 

    //Ariel Doing dis :)
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;
        ChannelFactory<DataServerInterface> foobFactory;
        private string loggedUser;
        public MainWindow()
        {
            InitializeComponent();
            //ChannelFactory<DataServerInterface> foobFactory;
            connectToServer();

            //Set this button to be invisible first
            Button_EnterChatRoom.Visibility = Visibility.Hidden;
            Button_EnterChatRoom.IsEnabled = false;
        }

        private void connectToServer()
        {
            NetTcpBinding tcpB = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
        }

        private void Buttong_Login_Click(object sender, RoutedEventArgs e)
        {
            string username = TextBox_Username.Text;
            if (username.Length == 0 ) 
            {
                Label_Alert.Content = "Username should not be empty!";
            }
            else
            {
                Label_Alert.Content = "";
                bool login = foob.addUser(username);
                if (login)
                {
                    Label_Alert.Content = "Login Successfully";
                    // Switch to another page with the username //

                    loggedUser = username;
                    Label_Logged.Content = ("Logged as: " + loggedUser);
                    Button_EnterChatRoom.IsEnabled = true;
                    Button_EnterChatRoom.Visibility = Visibility.Visible;

                }
                else
                {
                    Label_Alert.Content = "User Already Exists!";
                    // Stay at login page //
                }
            }
        }

        private void EnterChatRoom_Click(object sender, RoutedEventArgs e)
        {
            //Attempt to close the connection, so the other window can connect
            try
            {
                ((ICommunicationObject)foob).Close();
                foobFactory.Close();
                //Enter chatroom window
            }
            catch (Exception eR)
            {
                Console.WriteLine(eR.Message);
                connectToServer();
            }
        }
    }
}
