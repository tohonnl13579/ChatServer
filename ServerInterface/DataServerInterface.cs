using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Database;

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

        //SendPublicImage: able to send image data
        [OperationContract]
        void SendPublicImage(string roomName, string username, string imgData);

        //SendPublicTextFile: able to send textFile data
        [OperationContract]
        void SendPublicTextFile(string roomName, string username, string[] textFileData);

        [OperationContract]
        void SendPrivateMessage(string roomName, string fromUser, string toUser, string message);

        //SendPrivateImage: able to send image data
        [OperationContract]
        void SendPrivateImage(string roomName, string fromUser, string toUser, string imgData);

        //SendPrivateTextFIle: able to send textFile data
        [OperationContract]
        void SendPrivateTextFile(string roomName, string fromUser, string toUser, string[] textFileData);

        //WIP
        [OperationContract]
        List<Message> GetMSGs(string roomName, string username);

        [OperationContract]
        List<string> GetChatRoomList();

        [OperationContract]
        HashSet<string> GetUserOnline(string roomName);
    }
}
