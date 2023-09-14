using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.ServiceModel;
//using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using ChatServer;
using Database;
using ServerInterface;

namespace Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
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

        public void SendPublicImage(string roomName, string username, string imgData)
        {
            if (CheckRoomExisted(roomName))
            {
                db.SendImage(roomName, username, null, imgData);
            }
        }

        public void SendPublicTextFile(string roomName, string username, string[] textFileData)
        {
            if (CheckRoomExisted(roomName))
            {
                db.SendTextFile(roomName, username, null, textFileData);
            }
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

        public void SendPrivateImage(string roomName, string fromUser, string toUser, string imgData)
        {
            if (CheckRoomExisted(roomName))
            {
                if (CheckUserExistedInRoom(roomName, toUser))
                {
                    db.SendImage(roomName, fromUser, toUser, imgData);
                }
            }
        }

        public void SendPrivateTextFile(string roomName, string fromUser, string toUser, string[] textFileData)
        {
            if (CheckRoomExisted(roomName))
            {
                if (CheckUserExistedInRoom(roomName, toUser))
                {
                    db.SendTextFile(roomName, fromUser, toUser, textFileData);
                }
            }
        }

        //TESTING

        // FOR REFERENCE FOR ClientWPF
        // Just send a List<Message> to the client, After pain and suffering, this is the solution 
        // Refer to above Example Within //TESTING markers

        //CURRENTLY WIP
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<Message> GetMSGs(string roomName, string username)
        {
            List<Message> userMessages = new List<Message>();
            for (int i = 0; i < db.GetTotalRoom(); i++)
            {
                db.GetRoomNameByIndex(i, out string temproomname);
                if (roomName.Equals(temproomname))
                {
                    db.GetRoomMessages(i, out List<Message> messages);
                    //userMessages = messages;
                    for(int y = 0; y < messages.Count; y++)
                    {
                        if (messages[y].toUser == null) //If the Message is public
                        {
                            userMessages.Add(messages[y]);
                        }
                        else //If the Message is private
                        {
                            if ((messages[y].toUser).Equals(username) || (messages[y].fromUser).Equals(username)) //Check if the private message is issued to the user or is the sender
                            {
                                userMessages.Add(messages[y]);
                            }
                        }
                    }
                    break;
                }
            }
            return userMessages;
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









