using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ServerInterface
{
    [ServiceContract]
    public interface DataServerInterface
    {
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

        //SendPublicImgMessage: able to send image data
        [OperationContract]
        void SendPublicImgMessage(string roomName, string username, Bitmap imgData);

        [OperationContract]
        void SendPrivateMessage(string roomName, string fromUser, string toUser, string message);

        //SendPublicImgMessage: able to send image data
        [OperationContract]
        void SendPrivateImgMessage(string roomName, string fromUser, string toUser, Bitmap imgData);

        [OperationContract]
        List<string> GetMessages(string roomName, string username);

        [OperationContract]
        List<string> GetChatRoomList();

        [OperationContract]
        HashSet<string> GetUserOnline(string roomName);
    }
}
