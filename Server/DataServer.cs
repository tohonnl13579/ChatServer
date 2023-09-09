using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ChatServer;
using Database;
using ServerInterface;

namespace Server
{
    internal class DataServer : DataServerInterface
    {
        private ChatServer.Database db = new ChatServer.Database();

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

        public string CreateChatRoom(string roomName, string username)
        {
            string created;
            if (CheckRoomExisted(roomName))
            {
                created = null;
            }
            else
            {
                db.AddUserChatRoom(roomName, username);
                db.AddChatRoom(roomName);
                created = roomName;
            }
            return created;
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

        public void SendPublicMessage(string roomName, string username, string message)
        {
            if (CheckRoomExisted(roomName))
            {
                db.SendMessage(roomName, username, null, message);
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
                db.GetRoomNameByIndex(i, out string temproomname);
                if (roomName.Equals(temproomname))
                {
                    existed = true;
                    break;
                }
            }
            return existed;
        }

        private bool CheckUserExistedInRoom(string roomName, string username)
        {
            bool existed = false;
            db.GetUserListInRoom(roomName, out List<string> userList);
            for (int i=0; i < userList.Count; i++)
            {
                if (userList[i].Equals(username)) 
                { 
                    existed = true; 
                    break; 
                }
            }
            return existed;
        }
    }
}
