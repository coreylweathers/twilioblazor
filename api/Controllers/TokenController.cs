using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.Jwt;
using Twilio.Jwt.AccessToken;
using Twilio.Jwt.Client;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITwilioSettings _twilioSettings;
        private readonly ITwilioServiceSettings _twilioServiceSettings;
        private readonly TwilioCallSettings _twilioCallSettings;
               
        public TokenController(ILogger<TokenController> logger,IOptionsMonitor<TwilioAccountSettings> accountOptionsAccessor, IOptionsMonitor<TwilioChatSettings> chatOptionsAccessor, IOptionsMonitor<TwilioCallSettings> callOptionsAccessor)
        {
            _logger = logger;
            _twilioSettings = accountOptionsAccessor.CurrentValue;
            _twilioServiceSettings = chatOptionsAccessor.CurrentValue;
            _twilioCallSettings = callOptionsAccessor.CurrentValue;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> PostChatToken([FromForm] string identity = "Joe")
        {
            _logger.LogInformation("Getting the access token");

            if (!IsValidServiceSettings(_twilioServiceSettings))
            {
                var exception = new ApplicationException("Twilio Service Settings are not set correctly. Double check and retry your operation again");
                _logger.LogError(exception, $"{nameof(_twilioServiceSettings)} are not setting correctly");
                throw exception;
            }

            if (!IsValidSettings(_twilioSettings))
            {
                var exception = new ApplicationException("Twilio Settings are not set correctly. Double check and retry your operation again");
                _logger.LogError(exception, $"{nameof(_twilioSettings)} are not setting correctly");
                throw exception;
            }

            var chatGrant = new ChatGrant
            {
                ServiceSid = _twilioServiceSettings.ServiceSid
            };

            var hashGrants = new HashSet<IGrant>{ {chatGrant}};

            var token = new Token(_twilioSettings.AccountSid, _twilioServiceSettings.ApiKey, _twilioServiceSettings.ApiSecret, identity, grants: hashGrants);

            _logger.LogInformation("Got access token", token.ToJwt());

            return await Task.FromResult(new ContentResult { Content = token.ToJwt(), ContentType = "application/jwt", StatusCode = 200 });
        }

        [HttpPost("call")]
        public async Task<IActionResult> PostCallToken()
        {
            var scopes = new HashSet<IScope>
            {
                new OutgoingClientScope(_twilioCallSettings.AppSid),
                new IncomingClientScope("tester")
            };

            var capability = new ClientCapability(_twilioSettings.AccountSid, _twilioSettings.AuthToken, scopes: scopes);
            return await Task.FromResult(Content(capability.ToJwt(), "application/jwt"));
        }

        private bool IsValidSettings<T>(T settings) where T : ITwilioSettings
        {
            return !string.IsNullOrEmpty(settings.AccountSid) && !string.IsNullOrEmpty(settings.AuthToken);
        }

        private bool IsValidServiceSettings<T>(T settings) where T : ITwilioServiceSettings
        {
            return !string.IsNullOrEmpty(settings.ApiKey) && !string.IsNullOrEmpty(settings.ApiSecret) && !string.IsNullOrEmpty(settings.ServiceSid);
        }
    }
}