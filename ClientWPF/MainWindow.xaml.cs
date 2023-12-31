﻿using ServerInterface;
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
        private int portNumForClient;
        private List<string> userLogged;
        private List<string> userLog;
        public MainWindow()
        {
            InitializeComponent();

            //Set all the default state of all window elements where needed
            Warning_Label.Content = "";
            Username_TextBox.Text = "";
            LoggedIn_Label.Content = "Logged in as: ";
            Button_EnterChatRoom.Visibility = Visibility.Hidden;
            Button_EnterChatRoom.IsEnabled = false;
            portNumForClient = 8100;
            userLogged = new List<string>();
            userLog = new List<string>();

            connectToServer();
        }

        //Used to reconnect to the server
        private void connectToServer()
        {
            if (foobFactory != null)
            {
                foobFactory.Close();
            }
            NetTcpBinding tcpB = new NetTcpBinding();

            tcpB.CloseTimeout = new TimeSpan(0, 0, 10);
            tcpB.ReceiveTimeout = new TimeSpan(0, 0, 10);
            tcpB.SendTimeout = new TimeSpan(0, 0, 30);
            tcpB.MaxBufferPoolSize = 100000;
            tcpB.MaxReceivedMessageSize = 700000;
            tcpB.MaxBufferSize = 700000;
            tcpB.ReaderQuotas.MaxArrayLength = 10000;
            tcpB.ReaderQuotas.MaxDepth = 10;
            tcpB.ReaderQuotas.MaxBytesPerRead = 10000;
            tcpB.ReaderQuotas.MaxStringContentLength = 10000;

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
                    connectToServer();
                    //Enable the "Enter ChatRoom Button"
                    Warning_Label.Content = "Logged in successfully!";
                    loggedUser = username;
                    LoggedIn_Label.Content = "Logged in as: " + loggedUser;
                    Button_EnterChatRoom.Visibility= Visibility.Visible;
                    Button_EnterChatRoom.IsEnabled = true;
                }
                else
                {
                    if (checkIfUserExists(username))
                    {
                        connectToServer();
                        //Enable the "Enter ChatRoom Button"
                        Warning_Label.Content = "Welcome back " + username;
                        loggedUser = username;
                        LoggedIn_Label.Content = "Logged in as: " + loggedUser;
                        Button_EnterChatRoom.Visibility = Visibility.Visible;
                        Button_EnterChatRoom.IsEnabled = true;
                    }
                }
            }
        }

        //This function is called once user is logged in and they press the "Enter ChatRoom Button"
        private void enterChatRoom()
        {
            ChatRoomWindow chatRoomWindow = new ChatRoomWindow(loggedUser, portNumForClient, this);
            if (chatRoomWindow != null) {
                userLogged.Add(loggedUser);
                userLog.Add(loggedUser);
                chatRoomWindow.Show();
            }
            else
            {
                Warning_Label.Content = "Failed to switch windows";
            }
        }

        public void exitChatRoom(string user)
        {
            if (checkIfAlreadyIn(user))
            {
                for(int i = 0; i < userLogged.Count; i++)
                {
                    if (userLogged[i].Equals(user))
                    {
                        userLogged.Remove(user); 
                    }
                }
            }
        }

        private bool checkIfAlreadyIn(string user)
        {
            bool returnVal = false;
            foreach(string userInList in userLogged)
            {
                if (userInList.Equals(user))
                {
                    returnVal = true;
                }
            }
            return returnVal;
        }

        private bool checkIfUserExists(string user)
        {
            bool returnVal = false;
            foreach (string userInList in userLog)
            {
                if (userInList.Equals(user))
                {
                    returnVal = true;
                }
            }
            return returnVal;
        }

        private void EnterChatRoom_Click(object sender, RoutedEventArgs e)
        {
            //Button Func
            //Attempt to close the connection, so the other window can connect
            try
            {
                if (checkIfAlreadyIn(loggedUser))
                {
                    Warning_Label.Content = "User is already logged in!";
                }
                else
                {
                    enterChatRoom();
                }
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
