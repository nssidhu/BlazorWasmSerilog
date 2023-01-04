using BlazorWasmSerilog.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Logging.ClearProviders();


//https://nblumhardt.com/2019/11/serilog-blazor/
var loglevelSwitch = new LoggingLevelSwitch();
loglevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Error;

string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}]  {Message,-120:j}     {NewLine}{Exception}";
var loggingConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
     .MinimumLevel.ControlledBy(loglevelSwitch) // This has no effect on the Log Level
     .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
        // .ReadFrom.Configuration(builder.Configuration);
         //.Enrich.FromLogContext()
         //.WriteTo.Console(outputTemplate: LogTemplate)
        // .WriteTo.BrowserConsole(outputTemplate: LogTemplate);
         //.WriteTo.Console()
         .WriteTo.BrowserConsole();
         //.CreateLogger();

Log.Logger = loggingConfig.CreateLogger();

Serilog.Debugging.SelfLog.Enable(message => {
    // Do something with `message`
    System.Diagnostics.Debug.WriteLine(message);
    Console.WriteLine("error in configuring Serilog :" + message);
});






await builder.Build().RunAsync();


