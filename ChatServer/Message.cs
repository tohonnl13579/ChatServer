using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Message
    {
        public string fromUser;
        public string toUser;
        public string message;

        public Message()
        {
            fromUser = null;
            toUser = null;
            message = null;
        }
    }
}
