using Microsoft.AspNetCore.Components;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace client.Pages
{
    public partial class ChatMessages
    {
        [Inject]
        protected IHttpClientFactory HttpClientFactory { get; set; }

        public List<ChatMessage> Messages { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected async Task LoadMessages()
        {
            throw new NotImplementedException();
            // Call over to api to get messages

            // Add messages to List

            // Rerender component
        }
        
    }
}
