using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class ChatRoom
    {
        public string roomName;
        public HashSet<string> roomUsers;
        public List<Message> messages;

        public ChatRoom()
        {
            roomName = null;
            roomUsers = new HashSet<string>();
            messages = new List<Message>();
        }
    }
}
