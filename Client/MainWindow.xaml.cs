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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;
        public MainWindow()
        {
            InitializeComponent();
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
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
                }
                else
                {
                    Label_Alert.Content = "User Existed!";
                    // Stay at login page //
                }
            }
        }
    }
}
