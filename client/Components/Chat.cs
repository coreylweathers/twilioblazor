using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using shared.Constants;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace client.Pages
{
    public partial class Chat
    {
        [Inject]
        protected IHttpClientFactory HttpClientFactory { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected IConfiguration Config { get; set; }

        protected IList<ChatRoom> Channels { get; set; } = new List<ChatRoom>();

        private ChatMessages _chatMessages;

        private bool _isLogOnEnabled = true;
        private bool _isLogOffEnabled;
        private string _token;
        private string _username;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _token = await JSRuntime.InvokeAsync<string>(ChatConstants.GETTOKENMETHOD, nameof(_token));
                _isLogOnEnabled = !string.IsNullOrEmpty(_token);
            }

            _isLogOffEnabled = !_isLogOnEnabled;

            StateHasChanged();
        }

        protected async Task LogOn()
        {
            _token = await GetChatToken(_username);
            //await JSRuntime.InvokeVoidAsync("appMethods.saveToken", nameof(_token), _token);

            await InitializeTwilioChat();

            _isLogOnEnabled = false;
            _isLogOffEnabled = !_isLogOnEnabled;
            StateHasChanged();
        }

        private async Task<string> GetChatToken(string userName = "Joe")
        {
            var client = HttpClientFactory.CreateClient();
            var url = new Uri(Config[ChatConstants.TOKENURLPATH]);

            var postData = new Dictionary<string, string> { { "identity", userName } };
            var content = new FormUrlEncodedContent(postData);
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadAsStringAsync();
            return token;
        }

        protected async Task InitializeTwilioChat()
        {
            await JSRuntime.InvokeVoidAsync(ChatConstants.INITMETHOD, _token, DotNetObjectReference.Create(this));
        }

        protected async Task LogOff()
        {
            await Task.FromException(new NotImplementedException());
        }

        [JSInvokable]
        public async Task SaveChatChannels(string data)
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var rooms = JsonSerializer.Deserialize<ChatRoom[]>(data, opts);
            foreach (var room in rooms)
            {
                Channels.Add(room);
            }
            StateHasChanged();
            await Task.CompletedTask;
        }

    }
}