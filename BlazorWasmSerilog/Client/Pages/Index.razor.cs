using Microsoft.AspNetCore.Components;
using Serilog;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorWasmSerilog.Client.Pages
{
    public partial class Index : ComponentBase
    {
        
        //We can use either logger.
        private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext<Index>();

        [Inject]
        private ILogger<Index> _logger1 { get; set; }

        [Inject]
        private ILoggerFactory LoggerFactory { get; set; }

        protected override Task OnInitializedAsync()
        {
            var logger = LoggerFactory.CreateLogger<Counter>();


            logger.LogInformation("Info from factory");
            logger.LogWarning("Warning from factory");
            logger.LogError("Error from factory");

            _logger1.LogInformation("Info from Microsoft");
            _logger1.LogWarning("Warning from Microsoft");
            _logger1.LogError("Error from Microsoft");

            _logger.Information("Hello World");
            _logger.Warning("This is Warning");
            _logger.Error("This is error");


            //Log.CloseAndFlush();
            return base.OnInitializedAsync();
        }
    }
}
