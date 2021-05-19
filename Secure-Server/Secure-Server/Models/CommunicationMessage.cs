using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Server.Models
{
    public enum MessageCodes
    {
        Request,
        SuccessfulResponse,
        ErrorResponse,
        DisconnectResponse,
        UploadRequest,
        DownloadRequest,
        OwnFileSuccessfulDownload,
        OtherFileSuccessfulDownload,
        RequesterInfo
    }

    public class UploadMessage
    {
        public string message;
        public bool lastPacket;
    }

    public class RequesterInfo
    {
        public string filename;
        public string requesterUsername;
        public string requesterPublicKey;
    }

    class CommunicationMessage
    {
        public MessageCodes msgCode;
        public string topic;
        public string message;
    }
}
