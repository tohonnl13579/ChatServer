using ServerInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private Bitmap loadedImageData;
        private string loadedTextFileData;
        private int maxConnectAtt;
        public ChatRoomWindow(string user)
        {
            InitializeComponent();
            loggedUser = user;
            maxConnectAtt = 6;
            ChatRoomWarning_Label.Content = "HELLLO";
            TextBox_TextChatBox.Text = "";
            TextBox_PrivateMsgUser.Text = "";
            currChatRoom = null;
            loadedImageData = null;
            loadedTextFileData = null;
            ListView_ChatWindow.HorizontalContentAlignment = HorizontalAlignment.Left;
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
                ChatRoomWarning_Label.Content = "-- Total Connection Failure! --";
                connect = false;
            }
            else
            {
                maxConnectAtt--;
            }

            return connect;
        }

        //Event Handling
        //When user clicks on "Create room", it creates a room depending on the room name input
        private void CreateRoom_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            try
            {
                string roomName = "";
                if (currChatRoom != null)
                {
                    foob.LeaveChatRoom(currChatRoom, loggedUser);
                    //currChatRoom = null;
                }

                roomName = TextBox_CreateRoom.Text;
                if(roomName != "")
                {
                    string createdRoomName = foob.CreateChatRoom(roomName, loggedUser);
                    if(createdRoomName != null)
                    {
                        currChatRoom = createdRoomName;
                        Label_ChatRoom.Content = "Current Room: " + currChatRoom;
                        updateRooms();
                        updateMessages();
                        updateUsers();
                    }
                }
                else
                {
                    ChatRoomWarning_Label.Content = "Please input a valid room name";
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

        //When user clicks on "File" button, opens file explorer and uploads either a text or image file to send to the chat room
        private void FileSend_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            try
            {
                //Needs Implementing
                ChatRoomWarning_Label.Content = "";
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

        //When user clicks on any room under room list, the user will be redirected to that chat room
        private void RoomList_Select(object sender, SelectionChangedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            try
            {
                ListBoxItem item = (ListBoxItem)ListBox_RoomList.SelectedItem;
                if(currChatRoom != null)
                {
                    foob.LeaveChatRoom(currChatRoom, loggedUser);
                    //currChatRoom = null;
                    Label_ChatRoom.Content = "Select a room to enter...";
                }

                if (item != null)
                {
                    currChatRoom = foob.JoinChatRoom(item.Content.ToString(), loggedUser);
                    Label_ChatRoom.Content = "Current Room: " + currChatRoom;
                }
                updateMessages();
                updateUsers();
                TextBox_PrivateMsgUser.Text = "";
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

        //Additional functionality to send msg to a user privately, by clicking on the user
        private void UserList_Select(object sender, SelectionChangedEventArgs e)
        {
            string userChosen = (string)ListBox_UserList.SelectedItem;
            //ListBoxItem item = (ListBoxItem)ListBox_UserList.SelectedItem;
            if(userChosen != null)
            {
                TextBox_PrivateMsgUser.Text = userChosen;
            }
            else
            {
                TextBox_PrivateMsgUser.Text = "";
            }
        }

        //Send Message publicly in the room
        private void SendPublic_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            try
            {
                //Needs Implementing
                //check if there is any file loaded
                if(loadedImageData != null || loadedTextFileData != null)
                {

                    //reset to null once sent
                    loadedImageData = null;
                    loadedTextFileData = null;
                }
                else
                {

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

        //Send Message privately to someone within the same room
        private void SendPrivate_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            try
            {
                //Needs Implementing
                //check if there is any file loaded
                if (loadedImageData != null || loadedTextFileData != null)
                {

                    //reset to null once sent
                    loadedImageData = null;
                    loadedTextFileData = null;
                }
                else
                {

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

        //Updating GUI Lists
        private void updateRooms()
        {
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

        // Takes a List of type Object[] in which element is size 2
        // Object[2] format:
        // [string identifier , Bitmap imgData/string strData] strData being either a simple message or the contents of a text file
        // string identifier format:
        // for public msg: <username>: 
        // for private msg: <fromUser> -> <toUser>:
        private void updateMessages()
        {
            //ChatRoomWarning_Label.Content = "";
            try
            {
                //Change to foob.getMessages() once the server side updated to return object[] list
                List<object[]> messageData = new List<object[]>();

                ListView_ChatWindow.Items.Clear();
                if(currChatRoom != null)
                {
                    //COMMENT: Feels like coding JavaFx ngl...
                    for(int i = 0; i < messageData.Count; i++)
                    {
                        object[] data = messageData[i];
                        ListViewItem item = new ListViewItem();
                        StackPanel msgContainer = new StackPanel();

                        msgContainer.Orientation = Orientation.Vertical;
                        if (checkStrMsg(data))
                        {
                            string identifier = data[0].ToString();
                            string msg = data[1].ToString();

                            TextBlock identifierBlock = new TextBlock();
                            identifierBlock.Text = identifier;
                            identifierBlock.FontWeight = FontWeights.Bold;

                            TextBlock msgBlock = new TextBlock();
                            msgBlock.Text = msg;
                            msgBlock.FontSize = msgBlock.FontSize - 1;

                            msgContainer.Children.Add(identifierBlock);
                            msgContainer.Children.Add(msgBlock);
                        }
                        else if (checkImgMsg(data))
                        {
                            string identifier = data[0].ToString();
                            BitmapImage img = convertBitmapToImg((Bitmap)data[1]);

                            TextBlock identifierBlock = new TextBlock();
                            identifierBlock.Text = identifier;
                            identifierBlock.FontWeight= FontWeights.Bold;

                            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                            image.Source = img;
                            image.Stretch = Stretch.Uniform;
                            image.MaxHeight = 100;

                            msgContainer.Children.Add(identifierBlock);
                            msgContainer.Children.Add(image);
                        }
                        else
                        {
                            //Invalid object given
                            ChatRoomWarning_Label.Content = "Invalid data format recieved";
                            string identifier = data[0].ToString();
                            string error = "INVALID DATA FORMAT: Index " + i;

                            TextBlock identifierBlock = new TextBlock();
                            identifierBlock.Text = identifier;
                            identifierBlock.FontWeight = FontWeights.Bold;
                            //identifierBlock.Foreground = System.Windows.Media.Brushes.Crimson;

                            TextBlock errorBlock = new TextBlock();
                            errorBlock.Text = error;
                            errorBlock.FontWeight = FontWeights.Bold;
                            errorBlock.Foreground = System.Windows.Media.Brushes.Crimson;

                            msgContainer.Children.Add(identifierBlock);
                            msgContainer.Children.Add(errorBlock);
                        }
                        item.Content = msgContainer;
                        ListView_ChatWindow.Items.Add(item);
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

        //Helper method for updateMessages
        private bool checkStrMsg(Object[] messageObject)
        {
            bool isValid = false;
            if (messageObject[1] is string)
            {
                isValid = true;
            }
            return isValid;
        }

        //Helper method for updateMessages
        private bool checkImgMsg(Object[] messageObject)
        {
            bool isValid = false;
            if (messageObject[1] is Bitmap)
            {
                isValid = true;
            }
            return isValid;
        }

        //Used to convert a Bitmap object into a BitmapImage object for display
        private BitmapImage convertBitmapToImg(Bitmap bitmap)
        {
            BitmapImage bitmapImg = new BitmapImage();

            //Some complicated stuff, had to research a bunch of this :P
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the Bitmap to the memory stream in a specified image format
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Resetting the memory stream position to the beginning
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Create the BitmapImage from the memory stream
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = memoryStream;
                bitmapImg.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImg.EndInit();
            }

            return bitmapImg;
        }

        private void updateUsers()
        {
            //ChatRoomWarning_Label.Content = "";
            try
            {
                HashSet<string> userOnline = foob.GetUserOnline(currChatRoom);
                if (userOnline.Count > 0)
                {
                    ListBox_UserList.Items.Clear();
                    foreach (string user in userOnline)
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
