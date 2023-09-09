using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class ChatRoom
    {
        string roomName;
        List<string> roomUsers;

        ChatRoom()
        {
            roomName = null;
            roomUsers = new List<string>();
        }
    }
}
