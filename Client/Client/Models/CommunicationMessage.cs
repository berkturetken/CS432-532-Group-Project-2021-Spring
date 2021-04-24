﻿using System;
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
        ErrorResponse
    }

    public class Message
    {
        public MessageCodes msgCode;
        public string topic;
        public string message;
    }
}