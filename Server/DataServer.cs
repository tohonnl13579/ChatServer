using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer;
using ServerInterface;

namespace Server
{
    internal class DataServer : DataServerInterface
    {
        private ChatServer.Database db = new ChatServer.Database();

        public int GetNumEntries()
        {
            return db.GetNumRecords();
        }

        public void GetUsernameForEntry(int index, out string username)
        {
            db.GetUsernameByIndex(index, out username);
        }

        public bool addUser(string username)
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

        private bool CheckUserExisted(string username)
        {
            bool existed = false;
            for (int i=0; i<db.GetNumRecords(); i++) 
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
    }
}
