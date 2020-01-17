using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;

namespace MiniIndex.Core.Http
{
    public static class HttpClientConfigurationExtensions
    {
        public static IHttpClientBuilder ApplyResiliencePolicies(this IHttpClientBuilder builder)
        {
            Random jitter = new Random();

            AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreaker = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(x => x.StatusCode >= HttpStatusCode.InternalServerError)
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(10),
                    minimumThroughput: 6,
                    durationOfBreak: TimeSpan.FromMinutes(1));

            builder
                .AddTransientHttpErrorPolicy(transient => transient
                    .WaitAndRetryAsync(3, (retryCount) =>
                        TimeSpan.FromSeconds(Math.Pow(retryCount, 2))
                        + TimeSpan.FromMilliseconds(jitter.NextDouble() * 500d)))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30)))
                .AddPolicyHandler(circuitBreaker);

            return builder;
        }
    }
}