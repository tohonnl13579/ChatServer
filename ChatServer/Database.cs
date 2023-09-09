using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Database
    {
        List<string> usernames;
        List<ChatRoom> chatrooms;
        public Database() 
        {
            usernames = new List<string>();
            chatrooms = new List<ChatRoom>();
            ChatRoom room1 = new ChatRoom();
            room1.roomName = "ROOM 1";
            ChatRoom room2 = new ChatRoom();
            room2.roomName = "ROOM 2";
            ChatRoom room3 = new ChatRoom();
            room3.roomName = "ROOM 3";
            Message ms = new Message();
            ms.fromUser = "tohonnl";
            ms.toUser = null;
            ms.message = "Public!";
            Message ms2 = new Message();
            ms2.fromUser = "Admin";
            ms2.toUser = "tohonnl";
            ms2.message = "Private!";
            room3.messages.Add(ms);
            room3.messages.Add(ms2);
            chatrooms.Add(room1);
            chatrooms.Add(room2);
            chatrooms.Add(room3);
        }

        public void AddUser(string username)
        {
            usernames.Add(username);
        }

        public void AddChatRoom(string chatRoomName)
        {
            ChatRoom chatRoom = new ChatRoom();
            chatRoom.roomName = chatRoomName;
            chatrooms.Add(chatRoom);
        }

        public bool AddUserChatRoom(string roomName, string username)
        {
            bool result = false;
            for (int i=0; i<chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    result = chatrooms[i].roomUsers.Add(username);
                    break;
                }
            }
            return result;
        }

        public void RemoveUserChatRoom(string roomName, string username)
        {
            for (int i = 0; i < chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    chatrooms[i].roomUsers.Remove(username);
                    break;
                }
            }
        }

        public void SendMessage(string roomName, string fromUser, string toUser, string message)
        {
            for (int i = 0; i < chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    Message messageItem = new Message();
                    messageItem.fromUser = fromUser; 
                    messageItem.toUser = toUser;
                    messageItem.message = message;
                    chatrooms[i].messages.Add(messageItem);
                    break;
                }
            }
        }

        public HashSet<string> GetUserListInRoom(string roomName)
        {
            HashSet<string> userList = new HashSet<string>();
            for (int i = 0; i < chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    userList = chatrooms[i].roomUsers;
                    break;
                }
            }
            return userList;
        }

        public void GetUsernameByIndex(int index, out string username) 
        {
            username = usernames[index];
        }

        public void GetRoomNameByIndex(int index, out string roomName)
        {
            roomName = chatrooms[index].roomName;
        }

        public void GetRoomMessages(int roomID, out List<Message> messages)
        {
            messages = chatrooms[roomID].messages;
        }

        public int GetTotalUsers()
        {
            return usernames.Count;
        }

        public int GetTotalRoom()
        {
            return chatrooms.Count;
        }
    }
}
