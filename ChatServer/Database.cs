using Database;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public void SendImage(string roomName, string fromUser, string toUser, string base64ImageData)
        {
            for (int i = 0; i < chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    Message messageItem = new Message();
                    messageItem.fromUser = fromUser;
                    messageItem.toUser = toUser;
                    messageItem.imageData = base64ImageData;
                    chatrooms[i].messages.Add(messageItem);
                    break;
                }
            }
        }

        public void SendTextFile(string roomName, string fromUser, string toUser, string[] textFileData)
        {
            for (int i = 0; i < chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    Message messageItem = new Message();
                    messageItem.fromUser = fromUser;
                    messageItem.toUser = toUser;
                    messageItem.textFileData = textFileData;
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
