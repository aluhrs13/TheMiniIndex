using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileObjects.AgileMapper;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using MiniIndex.Persistence;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/overhead")]
    public class OverheadController : Controller
    {
        public OverheadController(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }
        private readonly TelemetryClient _telemetry;

        [HttpGet("seen")]
        public async Task<IActionResult> SeenUser(string since)
        {
            _telemetry.TrackEvent("UserSeen", new Dictionary<string, string> { { "TimeSince", since } });
            return Ok();
        }
    }
}