using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerInterface
{
    [ServiceContract]
    public interface DataServerInterface
    {
        // Login Page //
        [OperationContract]
        int GetNumEntries();
        [OperationContract]
        void GetUsernameForEntry(int index, out string username);
        [OperationContract]
        bool addUser(string username);
        // Login Page //

        // Logged In //
        //[OperationContract]
        //bool createChatRoom(string roomName);
        // Logged In //
    }
}
