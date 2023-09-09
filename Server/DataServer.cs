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

        public bool AddUser(string username)
        {
            bool added;
            if (CheckUserExisted(username))
            {
                added = false;
            }
            else
            {
                db.addUser(username);
                added = true;
            }
            return added;
        }

        public List<string> GetListOfChatRooms()
        {   
            List<string> roomList = new List<string>();
            for (int i=0; i<db.GetTotalRoom(); i++)
            {
                string roomName;
                db.GetRoomNameByIndex(i, out roomName);
                roomList.Add(roomName);
            }
            return roomList;
        }

        public string CreateChatRoom(string roomName)
        {
            string created;
            if (CheckRoomExisted(roomName))
            {
                created = null;
            }
            else
            {
                db.addChatRoom(roomName);
                created = roomName;
            }
            return created;
        }

        public string JoinChatRoom(string roomName, string username)
        {
            string nowRoomName = null;
            if (CheckRoomExisted(roomName))
            {
                db.addUserChatRoom(roomName, username);
                nowRoomName = roomName;
            }
            return nowRoomName;
        }

        public void SendMessage(string roomName, string username, string message)
        {
            if (CheckRoomExisted(roomName))
            {
                db.sendMessage(roomName, username + ": " + message);
            }
        }

        public List<string> GetPublicMessage(string roomName)
        {
            List<string> messages = new List<string>();
            for (int i=0; i<db.GetTotalRoom(); i++)
            {
                string temproomname;
                db.GetRoomNameByIndex(i, out temproomname);
                if (temproomname.Equals(roomName))
                {
                    db.GetRoomPublicMessages(i, out messages);
                    break;
                }
            }

            return messages;
        }

        private bool CheckUserExisted(string username)
        {
            bool existed = false;
            for (int i=0; i<db.GetTotalUsers(); i++) 
            {
                string tempusername = null;
                db.GetUsernameByIndex(i, out tempusername);
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
                string temproomname = null;
                db.GetRoomNameByIndex(i, out temproomname);
                if (roomName.Equals(temproomname))
                {
                    existed = true;
                    break;
                }
            }
            return existed;
        }
    }
}
