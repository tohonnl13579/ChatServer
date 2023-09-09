using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Database
    {
        // try commit //
        List<string> usernames;
        int index = 0;
        public Database() 
        {
            usernames = new List<string>();
        }

        public void addUser(string username)
        {
            usernames.Add(username);
        }

        public void GetUsernameByIndex(int index, out string username) 
        {
            username = usernames[index];
        }

        public int GetNumRecords()
        {
            return usernames.Count;
        }
    }
}
