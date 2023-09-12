using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChatServer;
using Database;
using ServerInterface;

namespace Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServer : DataServerInterface
    {
        private static ChatServer.Database db = new ChatServer.Database();

        public int GetNumEntries()
        {
            return db.GetTotalUsers();
        }

        public int GetNumChatRoom()
        {
            return db.GetTotalRoom();
        }

        public bool AddUser(string username)
        {
            bool added;
            if (CheckUserExisted(username))
            {
                added = false;
            }
            else
            {
                db.AddUser(username);
                added = true;
            }
            return added;
        }

        public string CreateChatRoom(string roomName, string username)
        {
            string newRoomName = null;
            if (!CheckRoomExisted(roomName))
            {
                db.AddChatRoom(roomName);
                db.AddUserChatRoom(roomName, username);
                newRoomName = roomName;
            }
            return newRoomName;
        }

        public string JoinChatRoom(string roomName, string username)
        {
            string nowRoomName = null;
            if (CheckRoomExisted(roomName))
            {
                db.AddUserChatRoom(roomName, username);
                nowRoomName = roomName;
            }
            return nowRoomName;
        }

        public void LeaveChatRoom(string roomName, string username)
        {
            if (CheckRoomExisted(roomName))
            {
                db.RemoveUserChatRoom(roomName, username);
            }
        }

        public void SendPublicMessage(string roomName, string username, string message)
        {
            if (CheckRoomExisted(roomName))
            {
                db.SendMessage(roomName, username, null, message);
            }
        }

        public void SendPublicImage(string roomName, string username, Bitmap imgData)
        {
            //Not implemented yet
        }

        public void SendPublicTextFile(string roomName, string username, string[] textFileData)
        {
            //Not implemented yet
        }

        public void SendPrivateMessage(string roomName, string fromUser, string toUser, string message) 
        {
            if (CheckRoomExisted(roomName))
            {
                if (CheckUserExistedInRoom(roomName, toUser))
                {
                    db.SendMessage(roomName, fromUser, toUser, message);
                }
            }
        }

        public void SendPrivateImage(string roomName, string fromUser, string toUser, Bitmap imgData)
        {
            //Not implemented yet
        }

        public void SendPrivateTextFile(string roomName, string fromUser, string toUser, string[] textFileData)
        {
            //Not implemented yet
        }

        //This is only for testing purposes, and may serve as a reference for changes to be made in the server
        public List<object[]> mockData()
        {
            List<object[]> objectList = new List<object[]>();

            object[] data1 = new object[2];
            data1[0] = "Ariel";
            data1[1] = "Hello this is a string messageData";

            object[] data2 = new object[2];
            data2[0] = "Ariel -> LeeTohOhn";
            data2[1] = "This is a private string messageData to user: LeeTohOhn";

            string[] mockTextFileData = new string[5];
            mockTextFileData[0] = "This";
            mockTextFileData[1] = "is an example";
            mockTextFileData[2] = "textFile data";
            mockTextFileData[3] = "for testing purposes";
            mockTextFileData[4] = "Thanks, Ariel";
            object[] data3 = new object[2];
            data3[0] = "Ariel";
            data3[1] = mockTextFileData;

            string[] mockTextFileData2= new string[5];
            mockTextFileData2[0] = "This";
            mockTextFileData2[1] = "is an example";
            mockTextFileData2[2] = "textFile data";
            mockTextFileData2[3] = "for testing purposes";
            mockTextFileData2[4] = "Thanks, Ariel";
            object[] data4 = new object[2];
            data4[0] = "Ariel -> LeeTohOhn";
            data4[1] = mockTextFileData;

            /*
            object[] data5 = new object[2];
            Bitmap bitmap = new Bitmap(500, 500);
            Graphics graph = Graphics.FromImage(bitmap);
            Rectangle ImageSize = new Rectangle(0, 0, 500, 500);
            graph.FillRectangle(Brushes.Gray, ImageSize);
            data5[0] = "Ariel";
            data5[1] = bitmap;
            */

            objectList.Add(data1);
            objectList.Add(data2);
            objectList.Add(data3);
            objectList.Add(data4);
            //objectList.Add(data5);

            return objectList;
        }

        // FOR REFERENCE FOR ClientWPF
        // Takes a List of type Object[] in which element is size 2
        // Object[2] format:
        // [string identifier , Bitmap imgData/string messageData/string[] textFileData]
        //
        // string identifier FORMAT:
        // for public msg: <username>: 
        // for private msg: <fromUser> -> <toUser>:

        public List<string> GetMessages(string roomName, string username)
        {
            List<string> messagesString = new List<string>();
            for (int i = 0; i < db.GetTotalRoom(); i++)
            {
                db.GetRoomNameByIndex(i, out string temproomname);
                if (roomName.Equals(temproomname))
                {
                    db.GetRoomMessages(i, out List<Message> messages);
                    for (int j=0; j<messages.Count; j++)
                    {
                        string toAdd;
                        if (username.Equals(messages[j].fromUser) || username.Equals(messages[j].toUser))
                        {
                            if (messages[j].toUser == null)
                            {
                                toAdd = "(Public) " + messages[j].fromUser  + ": " + messages[j].message;
                            }
                            else
                            {
                                toAdd = "(Private) " + messages[j].fromUser + " to " + messages[j].toUser + ": " + messages[j].message;
                            }
                        }
                        else
                        {
                            toAdd = "(Public) " + messages[j].fromUser + ": " + messages[j].message;
                        }
                        messagesString.Add(toAdd);
                    }
                    break;
                }
            }

            return messagesString;
        }

        public List<string> GetChatRoomList()
        {
            List<string> roomNameList = new List<string>();
            for (int i = 0; i < db.GetTotalRoom(); i++)
            {
                db.GetRoomNameByIndex(i, out string roomName);
                roomNameList.Add(roomName);
            }
            return roomNameList;
        }

        public HashSet<string> GetUserOnline(string roomName)
        {
            HashSet<string> userOnline = db.GetUserListInRoom(roomName);
            return userOnline;
        }

        private bool CheckUserExisted(string username)
        {
            bool existed = false;
            for (int i=0; i<db.GetTotalUsers(); i++) 
            {
                db.GetUsernameByIndex(i, out string tempusername);
                if (username.Equals(tempusername))
                {
                    existed = true;
                    break;
                }
            }
            return existed;
        }

        private bool CheckRoomExisted(string roomName)
        {
            bool existed = false;
            for (int i = 0; i < db.GetTotalRoom(); i++)
            {
                db.GetRoomNameByIndex(i, out string tempRoomName);
                if (roomName.Equals(tempRoomName))
                {
                    existed = true;
                    break;
                }
            }
            return existed;
        }

        private bool CheckUserExistedInRoom(string roomName, string username)
        {
            HashSet<string> userOnline = db.GetUserListInRoom(roomName);
            bool existed = userOnline.Contains(username);
            return existed;
        }
    }
}









