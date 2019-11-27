using System;
using System.Collections.Generic;
using System.Text;

namespace shared.Models
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
