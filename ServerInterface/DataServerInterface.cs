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
         * AddUser:
         * Add user into database after successfully login.
         * Return true if successfully login (username not existed).
         * Return false if failed to login (username existed).
         */
        [OperationContract]
        bool AddUser(string username);



        // FUNCTIONS FOR LOGGED IN PAGE //
        /* 
         * GetListOfChatRooms:
         * Get list of chat room name.
         */
        [OperationBehavior]
        List<string> GetListOfChatRooms();

        /* 
         * CreateChatRoom:
         * Create chat room with given room name.
         * Return the room name if the room is succefully created.
         * Return null if the chat room is failed to create (Room name used).
         */
        [OperationContract]
        string CreateChatRoom(string roomName);

        /* 
         * JoinChatRoom:
         * Join chat room with given room name.
         * Return the room name if successfully join into a room (Room exists).
         * Return null if the fail to join a room (Room not exist).
         */
        [OperationContract]
        string JoinChatRoom(string roomName, string username);

        /* 
         * SendPublicMessage:
         * Send public message in given room name.
         */
        void SendPublicMessage(string roomName, string username, string message);

        /* 
         * SendPrivatecMessage:
         * Send public message in given room name.
         */
        void SendPrivateMessage(string roomName, string fromUser, string toUser, string message);

        /* 
         * GetMessages:
         * Get all messages which is visible to current user with given room name and username.
         * Return the messages in the room if there is any.
         */
        [OperationContract]
        List<string> GetMessages(string roomName, string username);

    }
}
