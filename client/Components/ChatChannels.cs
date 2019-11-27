using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using shared.Models;
using shared.Constants;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.IO;
namespace client.Pages
{
    public partial class ChatChannels
    {
        [Inject]
        protected IHttpClientFactory HttpClientFactory { get; set; }
        [Inject]
        protected IConfiguration Configuration { get; set; }

        protected List<ChatRoom> Channels { get; set; }

        private ChatMessages chatMessages;

        protected override async Task OnInitializedAsync()
        {
            using var client = HttpClientFactory.CreateClient();

            var uri = new Uri($"{Configuration[ChatConstants.CHANNELURLPATH]}s");
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            Channels = ConvertFromJsonToObject<List<ChatRoom>>(data); 
        }

        protected async Task AddNewChannel(string channelName)
        {
            // VALIDATE THE CHANNELNAME TO MAKE SURE ITS NOT EMPTY OR NULL
            if (string.IsNullOrEmpty(channelName))
            {
                await Task.FromException(new ArgumentException($"The {nameof(channelName)} is either null or empty. Please provide an actual value"));
            }

            // CALL INTO THE API TO CREATE A NEW CHANNEL
            using var client = HttpClientFactory.CreateClient();

            var formData = new Dictionary<string, string>
            {
                { "channelName", channelName }
            };
            var content = new FormUrlEncodedContent(formData);
            var uri = new Uri($"{Configuration[ChatConstants.CHANNELURLPATH]}/create");
            var response = await client.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            // UPDATE THE LIST OF CHANNELS
            var data = await response.Content.ReadAsStringAsync();
            var channel = ConvertFromJsonToObject<ChatRoom>(data);

            Channels.Add(channel);


            // RE-RENDER THE COMPONENT WITH THE CURRENT CHANNEL REMAINING SELECTED AND THE NEW CHANNEL ADDED
            StateHasChanged();
        }

        private T ConvertFromJsonToObject<T>(string data)
        {
            var output = JsonSerializer.Deserialize<T>(data);
            return output;
        }
    }
}