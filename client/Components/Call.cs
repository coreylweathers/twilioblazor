using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shared.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.JSInterop;
using shared.Constants;

namespace client.Pages
{
    public partial class Call
    {

        private string _tokenUrl;
        private bool appSetupRun;

        protected bool IsDialDisabled { get; set; } = false;
        protected bool IsEndDisabled { get { return !IsDialDisabled; } }

        protected bool IsClearDisabled { get { return string.IsNullOrEmpty(Input.PhoneNumber); } }
        protected List<string> Logs { get; set; } = new List<string>();

        [Inject]
        protected IConfiguration Configuration {get;set;}
        [Inject]
        protected IHttpClientFactory HttpClientFactory { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        public CallInputModel Input { get; set; } = new CallInputModel();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !appSetupRun)
            {
                var token = await GetClientToken();
                await JSRuntime.InvokeVoidAsync(CallConstants.SETUPMETHOD, token);
                appSetupRun = true;
            }
        }

        private async Task<string> GetClientToken()
        {
            _tokenUrl = Configuration[ConfigConstants.CALLTOKENURLPATH];
            var uri = new Uri(_tokenUrl);

            using var client = HttpClientFactory.CreateClient();
            var response = await client.PostAsync(uri, null);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task InitiatePhoneCall()
        {
            IsDialDisabled = true;
            await LogMessage($"Calling the number {Input.PhoneNumber}");
            await JSRuntime.InvokeVoidAsync(CallConstants.PLACECALLMETHOD, Input.PhoneNumber);
            await LogMessage($"Called the number {Input.PhoneNumber}");
            StateHasChanged();
        }

        protected async Task EndPhoneCall()
        {
            IsDialDisabled = false;
            await LogMessage($"Ending the call to {Input.PhoneNumber}");
            await JSRuntime.InvokeVoidAsync(CallConstants.ENDCALLMETHOD);
            await LogMessage($"Ended the call to {Input.PhoneNumber}");
            StateHasChanged();

        }

        protected async Task ClearPhoneNumber()
        {
            await LogMessage("Clearing the phone number entry");
            Input.PhoneNumber = string.Empty;
            await LogMessage("Cleared the phone number entry");
            StateHasChanged();
        }

        public async Task LogMessage(string message)
        {
            Logs.Add($"{DateTimeOffset.Now} - {message}");
            await Task.CompletedTask;
        }
    }
}
