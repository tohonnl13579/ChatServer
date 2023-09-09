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
        }

        public void addUser(string username)
        {
            usernames.Add(username);
        }

        public void addChatRoom(string chatRoomName)
        {
            ChatRoom chatRoom = new ChatRoom();
            chatRoom.roomName = chatRoomName;
            chatrooms.Add(chatRoom);
        }

        public void addUserChatRoom(string roomName, string username)
        {
            for (int i=0; i<chatrooms.Count; i++)
            {
                if (chatrooms[i].roomName.Equals(roomName))
                {
                    chatrooms[i].roomUsers.Add(username);
                }
            }
        }

        public void GetUsernameByIndex(int index, out string username) 
        {
            username = usernames[index];
        }

        public void GetRoomNameByIndex(int index, out string roomName)
        {
            roomName = chatrooms[index].roomName;
        }

        public void GetRoomPublicMessages(int roomID, out List<string> publicMessages)
        {
            publicMessages = chatrooms[roomID].messages;
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
