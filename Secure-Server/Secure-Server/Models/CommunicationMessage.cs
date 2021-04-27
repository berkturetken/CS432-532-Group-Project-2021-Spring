﻿using System;
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
        DisconnectResponse
    }
    class CommunicationMessage
    {
        public MessageCodes msgCode;
        public string topic;
        public string message;
    }
}
