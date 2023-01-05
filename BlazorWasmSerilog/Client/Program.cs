using BlazorWasmSerilog.Client;
using BlazorWasmSerilog.Client.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Runtime.CompilerServices;
using static System.Collections.Specialized.BitVector32;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Logging.ClearProviders();

/*********************** Configure ***************************************/


//https://nblumhardt.com/2019/11/serilog-blazor/

var dynamicLogLevel = new DynamicSeriLoggingLevelSwitches();
dynamicLogLevel.MinimumLevel = LogEventLevel.Information;

//dynamicLogLevel.MicrosoftLevelSwitch.MinimumLevel = LogEventLevel.Error;
dynamicLogLevel.SQLServerRemoteLog.MinimumLevel = LogEventLevel.Error;

var loglevelSwitch = new LoggingLevelSwitch();
loglevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Warning;

//var configuration = new ConfigurationBuilder()
//        .SetBasePath(Directory.GetCurrentDirectory())
//        .AddJsonFile("appsettings.json")
//        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
//        .Build();

string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}]  {Message,-120:j}     {NewLine}{Exception}";



//This will enable use to use Serilog.Log.ForContext<Index>() from any page
Log.Logger = new LoggerConfiguration()
     .MinimumLevel.ControlledBy(dynamicLogLevel) 
     .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
     .Enrich.WithProperty("Source", "BlazorWebAssembly")
     .Enrich.WithProperty("AppName", "GetInLineV6")
     .WriteTo.BrowserConsole()
     .WriteTo.BrowserHttp($"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: dynamicLogLevel.SQLServerRemoteLog) //Need to do additional setup on Server side to recieve this log
     .Enrich.FromLogContext()
     .CreateLogger();

Serilog.Debugging.SelfLog.Enable(message => {
    // Do something with `message`
    System.Diagnostics.Debug.WriteLine(message);
    Console.WriteLine("error in configuring Serilog :" + message);
});

//This will redirect Logs from ILogger<Index> or ILoggerFactory LoggerFactory to Serilog from any Page
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

builder.Services.AddSingleton<DynamicSeriLoggingLevelSwitches>(dynamicLogLevel);

await builder.Build().RunAsync();


