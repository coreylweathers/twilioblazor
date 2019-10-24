using System;
using System.Collections.Generic;
using System.Text;

namespace shared.Models
{
    public interface ITwilioServiceSettings
    {
        string ApiKey { get; set; }
        string ApiSecret { get; set; }
        string ServiceSid { get; set; }
    }
}
