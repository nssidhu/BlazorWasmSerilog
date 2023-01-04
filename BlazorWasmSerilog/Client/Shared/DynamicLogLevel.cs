using Serilog.Core;
using Serilog.Events;

namespace BlazorWasmSerilog.Client.Shared
{
    public class DynamicSeriLoggingLevelSwitches : Serilog.Core.LoggingLevelSwitch
    {
        // Logging level switch that will be used for the "Microsoft" namespace
        public LoggingLevelSwitch MicrosoftLevelSwitch
            = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning);

        // Logging level switch that will be used for the "Microsoft.Hosting.Lifetime" namespace
        public LoggingLevelSwitch MicrosoftHostingLifetimeLevelSwitch
            = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

        public LoggingLevelSwitch SQLServerRemoteLog
           = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Error);
    }
}
