using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
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

    public class ClassifiedInfo
    {
        public string key;
        public string IV;
        public string originalFileName;
    }

    public class CommunicationMessage
    {
        public MessageCodes msgCode;
        public string topic;
        public string message;
    }
}
