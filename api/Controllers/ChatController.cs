using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using shared.Models;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Chat.V2.Service;
using System.Collections.Generic;
using System.Text.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ITwilioSettings _twilioSettings;
        private readonly TwilioChatSettings _twilioChatSettings;

        public ChatController(IOptionsMonitor<TwilioAccountSettings> twilioOptionsMonitor, IOptionsMonitor<TwilioChatSettings> chatOptionsAccessor)
        {
            _twilioSettings = twilioOptionsMonitor.CurrentValue;
            _twilioChatSettings = chatOptionsAccessor.CurrentValue;

            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
        }

        [HttpGet("channel")]
        public async Task<IActionResult> GetChannels()
        {
            var results = await ChannelResource.ReadAsync(
                pathServiceSid: _twilioChatSettings.ServiceSid);

            // TODO: Add code to handle paging on Channel results
            var channels = new List<ChatRoom>();
            foreach(var channel in results)
            {
                channels.Add(new ChatRoom 
                {
                    Sid = channel.Sid,
                    FriendlyName = channel.FriendlyName,
                    UniqueName = channel.UniqueName
                });
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            return new JsonResult(channels, options);
        }

        [HttpPost("channel/create")]
        public async Task<IActionResult> CreateChannel([FromForm] string channelName)
        {

            // TODO: Add exception handling to CHANNEL/CREATE method 

            var channel = await ChannelResource.CreateAsync(
                pathServiceSid: _twilioChatSettings.ServiceSid,
                friendlyName: channelName,
                uniqueName: channelName);

            var room = new ChatRoom
            {
                FriendlyName = channel.FriendlyName,
                Sid = channel.Sid,
                UniqueName = channel.UniqueName
            };

            var options = new System.Text.Json.JsonSerializerOptions
            { PropertyNameCaseInsensitive = true };
            return new JsonResult(room,options);
        }

        [HttpGet("channel/message")]
        public async Task<IActionResult> GetChannelMessages([FromQuery] string channelName)
        {
            throw new NotImplementedException();
        }
    }
}
