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
        // FUNCTIONS FOR LOGIN PAGE //
        /* 
         * GetNumEntries:
         * Return total number of users existed.
         */
        [OperationContract]
        int GetNumEntries();

        /* 
         * addUser:
         * Add user into database after successfully login.
         * Return true if successfully login (username not existed).
         * Return false if failed to login (username existed).
         */
        [OperationContract]
        bool AddUser(string username);



        // FUNCTIONS FOR LOGGED IN PAGE //
        [OperationBehavior]
        //void GetListOfChatRooms(out List<string> roomName, );
        /* 
         * createChatRoom:
         * Create chat room with given room name.
         * Return the room name if the room is succefully created.
         * Return null if the chat room is failed to create (Room name used).
         */
        [OperationContract]
        string CreateChatRoom(string roomName);

        /* 
         * joinChatRoom:
         * Join chat room with given room name.
         * Return the room name if successfully join into a room (Room exists).
         * Return null if the fail to join a room (Room not exist).
         */
        [OperationContract]
        string JoinChatRoom(string roomName, string username);

        void SendMessage(string roomName, string username, string message);

        /* 
         * GetPublicMessage:
         * Get chat room public messages with given room name.
         * Return the public messages in the room name if there is any.
         */
        [OperationContract]
        List<string> GetPublicMessage(string roomName);

    }
}
