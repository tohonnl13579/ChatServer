using ServerInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ClientWPF
{
    /*This is the window that contains the room selection and chat rooms*/
    public partial class ChatRoomWindow : System.Windows.Window
    {
        private DataServerInterface foob;
        private ChannelFactory<DataServerInterface> foobFactory;
        private string loggedUser, currChatRoom;
        private Bitmap loadedImageData;
        private string selectedFilePath;
        private string[] loadedTextFileData;
        private int portNum;
        List<string[]> textFileDataHolder;
        private MainWindow mainWindow;
        private bool online, roomSelected;

        private List<string> chatRooms = new List<string>();
        private HashSet<string> users = new HashSet<string>();
        private List<Database.Message> messages = new List<Database.Message>();

        private Thread t1;
        private Thread t2;
        private Thread t3;

        public ChatRoomWindow(string user, int portNum, MainWindow context)
        {
            InitializeComponent();
            ChatRoomWarning_Label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            loggedUser = user;
            ChatRoomWarning_Label.Content = "Welcome to the ChatRoom";
            TextBox_TextChatBox.Text = "";
            TextBox_PrivateMsgUser.Text = "";
            currChatRoom = null;
            loadedImageData = null;
            loadedTextFileData = null;
            selectedFilePath = null;
            textFileDataHolder = null;
            mainWindow = context;
            roomSelected = false;
            online = true;
            onlineAccess(1);
            ListView_ChatWindow.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            this.portNum = portNum;
            connectToServer();
            t1 = new Thread(new ThreadStart(updateRoomsT));
            t2 = new Thread(new ThreadStart(updateUsersT));
            t3 = new Thread(new ThreadStart(updateMessagesT));
            t1.Start();
            t2.Start();
            t3.Start();
        }

        //Update Rooms thread, to update with new Rooms being created for every client every 200 miliseconds
        private void updateRoomsT()
        {
            try
            {
                while (onlineAccess(-1))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        List<string> latestRooms = foob.GetChatRoomList();
                        if (!latestRooms.SequenceEqual(chatRooms))
                        {
                            updateRooms();
                        }
                    }));
                    Thread.Sleep(200);
                }
            } 
            catch (ThreadAbortException taE)
            {
                System.Windows.MessageBox.Show(taE.Message);
            }
            catch (ThreadInterruptedException tiE)
            {
                System.Windows.MessageBox.Show(tiE.Message);
            }
            catch(TaskCanceledException tcE)
            {
                System.Windows.MessageBox.Show(tcE.Message);
            }
        }

        //Update Users thread, to update with new users in each room for every client every 200 miliseconds
        private void updateUsersT()
        {
            try
            {
                while (onlineAccess(-1))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        HashSet<string> latestUsers = foob.GetUserOnline(currChatRoom);
                        if (!latestUsers.SequenceEqual(users))
                        {
                            updateUsers();
                        }
                    }));
                    Thread.Sleep(200);
                }
            }
            catch (ThreadAbortException taE)
            {
                System.Windows.MessageBox.Show(taE.Message);
            }
            catch (ThreadInterruptedException tiE)
            {
                System.Windows.MessageBox.Show(tiE.Message);
            }
            catch (TaskCanceledException tcE)
            {
                System.Windows.MessageBox.Show(tcE.Message);
            }
        }

        //Update Messages thread, to update with new messages for every client every 200 miliseconds
        private void updateMessagesT()
        {
            try
            {
                while (onlineAccess(-1))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (currChatRoom != null)
                        {
                            List<Database.Message> latestMessages = foob.GetMSGs(currChatRoom, loggedUser);
                            if (latestMessages.Count != messages.Count)
                            {
                                updateMessages();
                            }
                            else
                            {
                                for (int i = 0; i < latestMessages.Count; i++)
                                {
                                    if (latestMessages[i].fromUser != messages[i].fromUser || latestMessages[i].toUser != messages[i].toUser || latestMessages[i].message != messages[i].message || latestMessages[i].imageData != messages[i].imageData || latestMessages[i].textFileData != latestMessages[i].textFileData)
                                    {
                                        updateMessages();
                                        break;
                                    }
                                }
                            }
                        }
                    }));
                    Thread.Sleep(200);
                }
            }
            catch (ThreadAbortException taE)
            {
                System.Windows.MessageBox.Show(taE.Message);
            }
            catch (ThreadInterruptedException tiE)
            {
                System.Windows.MessageBox.Show(tiE.Message);
            }
            catch (TaskCanceledException tcE)
            {
                System.Windows.MessageBox.Show(tcE.Message);
            }
        }

        //Used to access the global variable 'online' to check if the client is still ongoing,
        //this also acts as a way to modify the state of the variable syncronoisly to prevent race condition
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool onlineAccess(int mod)
        {
            if(mod == -1)
            {
                //Returns the 'online' variable at its current state, so no modification
            }
            else if(mod == 0) 
            {
                online = false;
            }
            else if(mod == 1)
            {
                online = true;
            }
            return online;
        }

        //Used to reconnect to server for every server access as to clear memory
        private bool connectToServer()
        {
            if (foobFactory != null)
            {
                foobFactory.Close();
            }

            bool connect = true;
            NetTcpBinding tcpB = new NetTcpBinding();
            tcpB.CloseTimeout = new TimeSpan(0, 0, 5);
            tcpB.ReceiveTimeout = new TimeSpan(0, 0, 10);
            tcpB.SendTimeout = new TimeSpan(0, 0, 30);
            tcpB.MaxBufferPoolSize = 50000000; //50MB
            tcpB.MaxReceivedMessageSize = 50000000; //50MB
            tcpB.MaxBufferSize = 50000000; //50MB
            tcpB.ReaderQuotas.MaxArrayLength = 1000000;
            tcpB.ReaderQuotas.MaxDepth = 100;
            tcpB.ReaderQuotas.MaxBytesPerRead = 10000000;
            tcpB.ReaderQuotas.MaxStringContentLength = 1000000;

            string URL = "net.tcp://localhost:"+ portNum +"/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcpB, URL);
            foob = foobFactory.CreateChannel();
            Label_LoggedAs.Content = "Logged in as: " + loggedUser;

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
                    connectToServer();
                }

                roomName = TextBox_CreateRoom.Text;
                if(roomName != "")
                {
                    string createdRoomName = foob.CreateChatRoom(roomName, loggedUser);
                    connectToServer();
                    if (createdRoomName != null)
                    {
                        currChatRoom = createdRoomName;
                        Label_ChatRoom.Content = "Current Room: " + currChatRoom;
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
                ChatRoomWarning_Label.Content = "";
                fileBrowser();
                if (selectedFilePath == null)
                {
                    loadedImageData = null;
                    loadedTextFileData = null;
                    ChatRoomWarning_Label.Content = "No file was chosen...";
                    Button_FileSend.Content = "File";
                }
                else
                {
                    ChatRoomWarning_Label.Content = selectedFilePath;
                    imageAndTextFileReader();
                    Button_FileSend.Content = "File Loaded";
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

        //Mainly a helper function for FileSend_Click, user can choose what file to load from their device
        private void fileBrowser()
        {
            ChatRoomWarning_Label.Content = "";
            selectedFilePath = null;
            try
            {
                Thread fileDialogThread = new Thread(fileBrowserThread);
                fileDialogThread.SetApartmentState(ApartmentState.STA); // Set thread to Single-Threaded Apartment mode
                fileDialogThread.Start();

                //Pauses here as user chooses a file to load

                fileDialogThread.Join();
            }
            catch(ThreadAbortException tAE)
            {
                ChatRoomWarning_Label.Content = "Thread Aborted!: " + tAE.Message;
            }
            catch(Exception eR)
            {
                ChatRoomWarning_Label.Content = "Exception occured!: " + eR.Message;
            }
        }

        //This will be a thread that will run syncronously, which opens the file browser for the user to choose a file
        private void fileBrowserThread()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Setting filters, for this project, only text files and image files are accepted
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
        }

        //Attempts to read the file selected by the user
        private void imageAndTextFileReader()
        {
            loadedImageData = null;
            loadedTextFileData = null;
            try
            {
                try //First try to parse into Bitmap
                {
                    loadedImageData = new Bitmap(selectedFilePath);
                    loadedImageData.SetResolution(500, 500);
                }
                catch (ArgumentException aE) //if parsing fails
                {
                    loadedImageData = null;
                    try //Then try to get text file
                    {
                        loadedTextFileData = File.ReadAllLines(selectedFilePath);
                    }
                    catch (IOException ioE)
                    {
                        loadedTextFileData = null;
                        ChatRoomWarning_Label.Content = "Failed to Parse, Invalid input";
                    }
                }
            }
            catch (Exception eR)
            {
                loadedImageData = null;
                loadedTextFileData = null;
                ChatRoomWarning_Label.Content = "Exception occured: " + eR.Message;
            }
        }

        //Converts a Bitmap object into a Base64 byte string
        private string convertBitmapToStr(Bitmap bitmap)
        {
            string base64Str = null;
            if(bitmap != null)
            {
                try
                {
                    byte[] bitmapBytes;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Png);
                        bitmapBytes = stream.ToArray();
                    }
                    base64Str = Convert.ToBase64String(bitmapBytes);
                        
                }
                catch (Exception eR)
                {
                    ChatRoomWarning_Label.Content = "Failed to convert bitmap to string" + eR.Message;
                }
            }
            return base64Str;
        }

        //Converts a base64 byte string to a bitmap object
        private Bitmap convertStrToBitmap(string base64)
        {
            Bitmap bitmap = null;
            if (base64 != null)
            {
                try
                {
                    //Convert Base64 string to byte[]
                    byte[] byteBuffer = Convert.FromBase64String(base64);
                    MemoryStream memoryStream = new MemoryStream(byteBuffer);

                    memoryStream.Position = 0;

                    bitmap = (Bitmap)Bitmap.FromStream(memoryStream);

                    memoryStream.Close();
                    memoryStream = null;
                    byteBuffer = null;

                    return bitmap;
                }
                catch(Exception eR)
                {
                    ChatRoomWarning_Label.Content = "Failed to convert bse64 string to bitmap" + eR.Message;
                }
            }
            return bitmap;
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
                    connectToServer();
                    Label_ChatRoom.Content = "Select a room to enter...";
                }

                if (item != null)
                {
                    currChatRoom = foob.JoinChatRoom(item.Content.ToString(), loggedUser);
                    connectToServer();
                    Label_ChatRoom.Content = "Current Room: " + currChatRoom;
                    roomSelected = true;
                }
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
            if (roomSelected)
            {
                try
                {
                    string message = TextBox_TextChatBox.Text;
                    //check if there is any file loaded
                    if (loadedImageData != null || loadedTextFileData != null)
                    {
                        //If any file presently loaded, if there is any given message, it will appear on top of the file
                        if (message == null || message.Equals(""))
                        {
                            if (loadedImageData != null)
                            {
                                foob.SendPublicImage(currChatRoom, loggedUser, convertBitmapToStr(loadedImageData));
                                connectToServer();
                                loadedImageData.Dispose();
                            }
                            else
                            {
                                foob.SendPublicTextFile(currChatRoom, loggedUser, loadedTextFileData);
                                connectToServer();
                            }
                        }
                        else
                        {
                            foob.SendPublicMessage(currChatRoom, loggedUser, message);
                            connectToServer();
                            if (loadedImageData != null)
                            {
                                foob.SendPublicImage(currChatRoom, loggedUser, convertBitmapToStr(loadedImageData));
                                connectToServer();
                                loadedImageData.Dispose();
                            }
                            else
                            {
                                foob.SendPublicTextFile(currChatRoom, loggedUser, loadedTextFileData);
                                connectToServer();
                            }
                            TextBox_TextChatBox.Text = "";
                        }

                        //reset to null once sent
                        loadedImageData = null;
                        loadedTextFileData = null;
                        Button_FileSend.Content = "File";
                    }
                    else
                    {
                        //If any file is not loaded, then just send a simple string message
                        if (message == null || message.Equals(""))
                        {
                            ChatRoomWarning_Label.Content = "Text bar is empty";
                        }
                        else
                        {
                            foob.SendPublicMessage(currChatRoom, loggedUser, message);
                            connectToServer();
                            TextBox_TextChatBox.Text = "";
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
            else
            {
                ChatRoomWarning_Label.Content = "Select a room first please...";
            }
        }

        //Prevents user from closing the window forcefully without proper log out
        private void window_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (onlineAccess(-1))
            {
                e.Cancel = true;
                ChatRoomWarning_Label.Content = "Log Out Properly Please...";
            }
        }

        //Log Out button
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            //Closing of threads
            onlineAccess(0);
            t1.Join();
            t2.Join();
            t3.Join();

            mainWindow.exitChatRoom(loggedUser);
            foobFactory.Close();
            this.Close();
        }

        //Send Message privately to someone within the same room
        private void SendPrivate_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            if (roomSelected)
            {
                try
                {
                    if ((TextBox_PrivateMsgUser.Text).Equals(loggedUser))
                    {
                        ChatRoomWarning_Label.Content = "You cannot send a private message to yourself";
                    }
                    else if ((TextBox_PrivateMsgUser.Text).Equals("") || (TextBox_PrivateMsgUser.Text) == null)
                    {
                        ChatRoomWarning_Label.Content = "Enter an existing user to send to...";
                    }
                    else if (userExists(TextBox_PrivateMsgUser.Text))
                    {
                        string message = TextBox_TextChatBox.Text;
                        //check if there is any file loaded
                        if (loadedImageData != null || loadedTextFileData != null)
                        {
                            //If any file presently loaded, if there is any given message, it will appear on top of the file
                            if (message == null || message.Equals(""))
                            {
                                if (loadedImageData != null)
                                {
                                    foob.SendPrivateImage(currChatRoom, loggedUser, TextBox_PrivateMsgUser.Text, convertBitmapToStr(loadedImageData));
                                    connectToServer();
                                    loadedImageData.Dispose();
                                }
                                else
                                {
                                    foob.SendPrivateTextFile(currChatRoom, loggedUser, TextBox_PrivateMsgUser.Text, loadedTextFileData);
                                    connectToServer();
                                }
                            }
                            else
                            {
                                foob.SendPrivateMessage(currChatRoom, loggedUser, TextBox_PrivateMsgUser.Text, message);
                                if (loadedImageData != null)
                                {
                                    foob.SendPrivateImage(currChatRoom, loggedUser, TextBox_PrivateMsgUser.Text, convertBitmapToStr(loadedImageData));
                                    connectToServer();
                                    loadedImageData.Dispose();
                                }
                                else
                                {
                                    foob.SendPrivateTextFile(currChatRoom, loggedUser, TextBox_PrivateMsgUser.Text, loadedTextFileData);
                                    connectToServer();
                                }
                                TextBox_TextChatBox.Text = "";
                            }

                            //reset to null once sent
                            loadedImageData = null;
                            loadedTextFileData = null;
                            Button_FileSend.Content = "File";
                        }
                        else
                        {
                            //If any file is not loaded, then just send a simple string message
                            if (message == null || message.Equals(""))
                            {
                                ChatRoomWarning_Label.Content = "Text bar is empty";
                            }
                            else
                            {
                                foob.SendPrivateMessage(currChatRoom, loggedUser, TextBox_PrivateMsgUser.Text, message);
                                connectToServer();
                                TextBox_TextChatBox.Text = "";
                            }
                        }
                    }
                    else
                    {
                        ChatRoomWarning_Label.Content = "This user does not exist!";
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
            else
            {
                ChatRoomWarning_Label.Content = "Select a room first please...";
            }
        }

        private bool userExists(string username)
        {
            bool exists = false;
            HashSet<string> userOnline = foob.GetUserOnline(currChatRoom);
            connectToServer();
            foreach (string user in userOnline)
            {
                if (user.Equals(username))
                {
                    exists = true;
                }
            }
            return exists;
        }

        //Updating GUI Lists
        private void updateRooms()
        {
            ListBox_RoomList.Items.Clear();
            try
            {
                chatRooms = foob.GetChatRoomList();
                connectToServer();
                for (int i = 0; i < chatRooms.Count; i++)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = chatRooms[i];
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

        //This function will convert Message data objects into a list of object[]
        private List<object[]> getMsgData()
        {
            List<object[]> objList = null;
            if(currChatRoom == null)
            {
                //Invalid, so return null
            }
            else
            {
                objList = new List<object[]>();
                messages = foob.GetMSGs(currChatRoom, loggedUser);
                connectToServer();
                int count = messages.Count;

                for (int i = 0; i < count; i++)
                {
                    object[] obj = new object[2];
                    Database.Message msg = messages[i];
                    if (msg.toUser == null)
                    {
                        String identifier = msg.fromUser;
                        obj[0] = identifier;
                    }
                    else
                    {
                        String identifier = msg.fromUser + " -> " + msg.toUser;
                        obj[0] = identifier;
                    }

                    if (msg.message != null)
                    {
                        obj[1] = msg.message;
                    }
                    else if (msg.imageData != null)
                    {
                        obj[1] = convertStrToBitmap(msg.imageData);
                    }
                    else if (msg.textFileData != null)
                    {
                        obj[1] = msg.textFileData;
                    }
                    objList.Add(obj);
                }
            }
            
            return objList;
        }

        // Takes a List of type Object[] in which element is size 2
        // Object[2] format:
        // [string identifier , Bitmap imgData/string messageData/string[] textFileData]
        //
        // string identifier FORMAT:
        // for public msg: <username>: 
        // for private msg: <fromUser> -> <toUser>:

        private void updateMessages()
        {
            try
            {
                List<object[]> messageData = null;
                messageData = getMsgData();
                textFileDataHolder = new List<string[]>();
                int buttonIDCounter = 1;

                ListView_ChatWindow.Items.Clear();
                if(messageData != null)
                {
                    if (currChatRoom != null)
                    {
                        for (int i = 0; i < messageData.Count; i++)
                        {
                            object[] data = messageData[i];
                            System.Windows.Controls.ListViewItem item = new System.Windows.Controls.ListViewItem();
                            StackPanel msgContainer = new StackPanel();

                            msgContainer.Orientation = System.Windows.Controls.Orientation.Vertical;

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
                                identifierBlock.FontWeight = FontWeights.Bold;

                                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                                image.Source = img;
                                image.Stretch = Stretch.Uniform;
                                image.MaxHeight = 100;

                                msgContainer.Children.Add(identifierBlock);
                                msgContainer.Children.Add(image);
                            }
                            else if (checkFileMsg(data))
                            {
                                string identifier = data[0].ToString();
                                string[] textFileData = (string[])data[1];

                                TextBlock identifierBlock = new TextBlock();
                                identifierBlock.Text = identifier;
                                identifierBlock.FontWeight = FontWeights.Bold;

                                System.Windows.Controls.Button Button_linkToFile = new System.Windows.Controls.Button();
                                Button_linkToFile.Height = 40;
                                Button_linkToFile.Width = 100;
                                Button_linkToFile.Content = "Link to File";
                                Button_linkToFile.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                                Button_linkToFile.FontWeight = FontWeights.Bold;
                                String stringID = "";
                                for(int x = 0; x < buttonIDCounter; x++)
                                {
                                    stringID += "c";
                                }
                                Button_linkToFile.Name = stringID;
                                buttonIDCounter++;
                                textFileDataHolder.Add(textFileData);
                                Button_linkToFile.Click += new RoutedEventHandler(linkToFileButton_Click);

                                msgContainer.Children.Add(identifierBlock);
                                msgContainer.Children.Add(Button_linkToFile);
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
                    else
                    {
                        ChatRoomWarning_Label.Content = "currRoom is NULL";
                    }
                }
                else
                {
                    ChatRoomWarning_Label.Content = "messageData is NULL";
                }
            }
            catch (CommunicationException cE)
            {
                ChatRoomWarning_Label.Content = "Connection Lost!: " + cE.Message;
                System.Windows.MessageBox.Show(cE.StackTrace);
                connectToServer();
            }
            catch(InvalidCastException iCE)
            {
                ChatRoomWarning_Label.Content = "InvalidCastException occured: " + iCE.Message;
            }
            catch (Exception eR)
            {
                ChatRoomWarning_Label.Content = "Exception occured: " + eR.Message;
            }
        }

        private void linkToFileButton_Click(object sender, RoutedEventArgs e)
        {
            ChatRoomWarning_Label.Content = "";
            System.Windows.Controls.Button buttonAccess = (System.Windows.Controls.Button)sender;
            if(buttonAccess != null)
            {
                int buttonID = buttonAccess.Name.Length;
                
                FileViewer fileViewerWindow = new FileViewer(textFileDataHolder[buttonID-1]);
                if(fileViewerWindow != null)
                {
                    fileViewerWindow.Show();
                }
                else
                {
                    ChatRoomWarning_Label.Content = "Failed to open file";
                }
                
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

        //Helper method for updateMessages
        private bool checkFileMsg(Object[] messageObject)
        {
            bool isValid = false;
            if (messageObject[1] is string[])
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
                // Save the Bitmap to the memory stream as a .png format
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                memoryStream.Seek(0, SeekOrigin.Begin);

                // Creating the BitmapImage object from the memory stream
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
                users = foob.GetUserOnline(currChatRoom);
                connectToServer();
                if (users.Count > 0)
                {
                    ListBox_UserList.Items.Clear();
                    foreach (string user in users)
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
