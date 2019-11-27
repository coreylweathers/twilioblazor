using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using shared.Models;
using System.Threading.Tasks;
using Twilio.TwiML;
using Twilio.Types;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallController : ControllerBase
    {
        private readonly TwilioCallSettings _twilioCallSettings;

        public CallController(IOptionsMonitor<TwilioCallSettings> callOptionsAccessor)
        {
            _twilioCallSettings = callOptionsAccessor.CurrentValue;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string phone)
        {
            var destination = !phone.StartsWith('+') ? $"+{phone}" : phone;

            var response = new VoiceResponse();
            var dial = new Twilio.TwiML.Voice.Dial
            {
                CallerId = _twilioCallSettings.PhoneNumber
            };
            dial.Number(new PhoneNumber(destination));

            response.Append(dial);

            return await Task.FromResult(Content(response.ToString(), "application/xml"));
        }
    }
}
