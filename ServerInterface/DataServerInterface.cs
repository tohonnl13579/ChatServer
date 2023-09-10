﻿using System;
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
        [OperationContract]
        void updateField();

        [OperationContract]
        int getField();
        // FUNCTIONS FOR LOGIN PAGE //
        [OperationContract]
        bool AddUser(string username);



        // FUNCTIONS FOR LOGGED IN PAGE //
        [OperationContract]
        int GetNumEntries();

        [OperationContract]
        int GetNumChatRoom();

        [OperationContract]
        string CreateChatRoom(string roomName, string username);

        [OperationContract]
        string JoinChatRoom(string roomName, string username);

        [OperationContract]
        void LeaveChatRoom(string roomName, string username);

        [OperationContract]
        void SendPublicMessage(string roomName, string username, string message);

        [OperationContract]
        void SendPrivateMessage(string roomName, string fromUser, string toUser, string message);

        [OperationContract]
        List<string> GetMessages(string roomName, string username);

        [OperationContract]
        List<string> GetChatRoomList();

        [OperationContract]
        HashSet<string> GetUserOnline(string roomName);
    }
}
