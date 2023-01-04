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


//https://nblumhardt.com/2019/11/serilog-blazor/
var loglevelSwitch = new LoggingLevelSwitch();
loglevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;

string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}]  {Message,-120:j}     {NewLine}{Exception}";
var logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(loglevelSwitch) // This has no effect on the Log Level
     .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
      //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
         //.ReadFrom.Configuration(builder.Configuration)
         //.Enrich.FromLogContext()
         //.WriteTo.Console(outputTemplate: LogTemplate)
         //.WriteTo.BrowserConsole(outputTemplate: LogTemplate)
         .WriteTo.Console()
         .WriteTo.BrowserConsole()
         .CreateLogger();

Serilog.Debugging.SelfLog.Enable(message => {
    // Do something with `message`
    System.Diagnostics.Debug.WriteLine(message);
    Console.WriteLine("error in configuring Serilog :" + message);
});

builder.Logging.ClearProviders();


//Only this line changes the Log Level
//builder.Logging.SetMinimumLevel(LogLevel.Error);

builder.Logging.AddSerilog(logger,dispose:true);
//builder.Logging.AddSerilog(logger);
//builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


await builder.Build().RunAsync();


