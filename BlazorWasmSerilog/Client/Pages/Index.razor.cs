using Microsoft.AspNetCore.Components;
using Serilog;
using System;

namespace BlazorWasmSerilog.Client.Pages
{
    public partial class Index : ComponentBase
    {
        
        private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext<Index>();
        protected override Task OnInitializedAsync()
        {

            _logger.Information("Hello World");
            _logger.Warning("This is Warning");
            _logger.Error("This is error");


            //Log.CloseAndFlush();
            return base.OnInitializedAsync();
        }
    }
}
