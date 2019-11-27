using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace client.Pages
{
    public partial class Index
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        protected async Task TestClick()
        {
            await JSRuntime.InvokeVoidAsync("methods.init", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async Task TestThis(string data)
        {
            Console.WriteLine(data);
            await Task.CompletedTask;
        }
    }
}
