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
        public List<string> roomUsers;
        public List<string> messages;

        public ChatRoom()
        {
            roomName = null;
            roomUsers = new List<string>();
            messages = new List<string>();
        }
    }
}
