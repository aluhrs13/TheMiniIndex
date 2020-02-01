using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;


namespace MiniIndex
{
    public class AppInsightsFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public AppInsightsFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (!OKtoSend(item)) { return; }

            Next.Process(item);
        }

        private bool OKtoSend(ITelemetry item)
        {
            var traceTelemetry = item as TraceTelemetry;
            if (traceTelemetry == null)
            {
                return true;
            }

            if (traceTelemetry.SeverityLevel == SeverityLevel.Verbose || traceTelemetry.SeverityLevel == SeverityLevel.Information)
            {
                return false;
            }

            return true;
        }
    }
}

