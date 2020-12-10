using Microsoft.ApplicationInsights.AspNetCore.TelemetryInitializers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex
{
    public class TelemetryEnrichment : TelemetryInitializerBase
    {
        public TelemetryEnrichment(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override void OnInitializeTelemetry(HttpContext platformContext, RequestTelemetry requestTelemetry, ITelemetry telemetry)
        {
            telemetry.Context.User.AuthenticatedUserId =
                platformContext.User?.Identity.Name ?? string.Empty;
        }
    }
}
