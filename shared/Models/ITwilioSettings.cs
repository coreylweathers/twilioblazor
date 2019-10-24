using System;
using System.Collections.Generic;
using System.Text;

namespace shared.Models
{
    public interface ITwilioSettings
    {
        string AccountSid { get; set; }
        string AuthToken { get; set; }
    }
}
