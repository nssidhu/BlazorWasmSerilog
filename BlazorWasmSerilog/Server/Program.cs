using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

//"expression": "Contains(SourceContext, 'AspNetCoreSerilogDemo.TestLogApi') and @Level = 'Information'"

//    var expr = "@Level = 'Information' and AppId is not null and Items[?] like 'C%'";

//https://github.com/serilog-mssql/serilog-sinks-mssqlserver/blob/dev/README.md
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
               {
                   new SqlColumn("UserName", SqlDbType.VarChar),
                   new SqlColumn("AppName", SqlDbType.VarChar),
                   new SqlColumn("Source", SqlDbType.VarChar),
                   new SqlColumn("SourceContext", SqlDbType.VarChar,dataLength:500),
                   new SqlColumn("InstanceId", SqlDbType.VarChar),
                   new SqlColumn("ConnectionId", SqlDbType.VarChar),
                   new SqlColumn("Origin", SqlDbType.VarChar)
               }
}; //through this coulmnsOptions we can dynamically  add custom columns which we want to add in database

columnOptions.Store.Add(StandardColumn.LogEvent); //uses JSON Format
columnOptions.LogEvent.DataLength = 4000;
columnOptions.LogEvent.ExcludeStandardColumns = true;
columnOptions.LogEvent.ExcludeAdditionalProperties = true; //If we have Column with same name(case Sensitive) than the value will not be stored in Logevent
columnOptions.Store.Remove(StandardColumn.Properties); //uses XML format
var loglevelSwitch = new LoggingLevelSwitch();
loglevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;

//Microsoft.AspNetCore.Routing.EndpointMiddleware
//Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker
//Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor
//Microsoft.AspNetCore.Routing.EndpointMiddleware

string[] ExludeFilter = { "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                         "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker",
                         "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor",
                         "Microsoft.AspNetCore.Routing.EndpointMiddleware",
                         "Microsoft.Hosting.Lifetime",
                         "Microsoft.AspNetCore.Hosting.Diagnostics",
                         "Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware",
                         "Serilog.AspNetCore.RequestLoggingMiddleware"
};


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(loglevelSwitch)
     //.Filter.ByExcluding("SourceContext like 'Microsoft.Hosting.Lifetime%' or SourceContext like 'Microsoft.AspNetCore.Hosting.Diagnostics%' or SourceContext like 'Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware%' or SourceContext like 'Serilog.AspNetCore.RequestLoggingMiddleware%'")
    .Filter.ByExcluding(evt =>
    {
        LogEventPropertyValue name;
        evt.Properties.TryGetValue("SourceContext", out name);
        string sourcontextValue = (name as ScalarValue)?.Value as string;
        var valueFound = ExludeFilter.Contains(sourcontextValue);
        return valueFound;
        //return evt.Properties.TryGetValue("SourceContext", out name) &&
        //    (name as ScalarValue)?.Value as string == "World";
    })
   
    .WriteTo.Console()
    .WriteTo.MSSqlServer("Server=tcp:getinlinedev-srv.database.windows.net,1433;Initial Catalog=GetinLine-Dev-DB;Persist Security Info=False;User ID=nssidhu;Password=Rajandar1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", 
             sinkOptions: new MSSqlServerSinkOptions 
             { TableName = "LogNNN", AutoCreateSqlTable =true }
               , null, null, LogEventLevel.Information, null, columnOptions: columnOptions,  null, null)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// If needed, Clear default providers
builder.Logging.ClearProviders();

//https://github.com/serilog/serilog-aspnetcore
builder.Host.UseSerilog(); // will redirect all log events through your Serilog pipeline.

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

//https://github.com/serilog/serilog-aspnetcore
app.UseSerilogIngestion(); // will setup http listner (/ingest) to capture logs coming from client serilog emitter
//app.UseSerilogRequestLogging(); //setup the middleware
app.UseSerilogRequestLogging(options =>
{
        
    // Attach additional properties to the request completion event
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var userName = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : "Guest1"; //Gets user Name from user Identity 
        LogContext.PushProperty("UserName", userName); //Push user in LogContext;  
        diagnosticContext.Set("UserID", "nssidhu@yahoo.com");
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("ClientIP", httpContext?.Connection?.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserAgent", httpContext?.Request.Headers["User-Agent"].FirstOrDefault());
       
    };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


// below code is needed to get User name for Log             
app.Use(async (httpContext, next) =>
{
    var userName = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : "Guest"; //Gets user Name from user Identity  
    LogContext.PushProperty("UserName", userName); //Push user in LogContext;  
    await next.Invoke();
}
);

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
