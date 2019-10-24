using System;
using System.Collections.Generic;
using System.Text;

namespace shared.Models
{
    public class TwilioAccountSettings : ITwilioSettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
    }
}
