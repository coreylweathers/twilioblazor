using System;
using System.Collections.Generic;
using System.Text;

namespace shared.Models
{
    public class TwilioChatSettings : ITwilioServiceSettings
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ServiceSid { get; set; }
    }
}
