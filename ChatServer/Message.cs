﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Message
    {
        public string fromUser;
        public string toUser;
        public string message;
        public Bitmap imageData;
        public string[] textFileData;

        public Message()
        {
            fromUser = null;
            toUser = null;
            message = null;
            imageData = null;
            textFileData = null;
        }
    }
}
