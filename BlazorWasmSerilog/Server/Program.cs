using Microsoft.AspNetCore.ResponseCompression;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
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
app.UseSerilogRequestLogging(); //setup the middleware

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

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
