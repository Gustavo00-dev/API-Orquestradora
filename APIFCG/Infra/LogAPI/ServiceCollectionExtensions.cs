using System;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace APIFCG.Infra.LogAPI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddTransient<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }

        public static IServiceCollection AddElasticLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("ElasticSettings").Get<ElasticSettings>() ?? new ElasticSettings();

            services.AddSingleton(settings);

            // configure HttpClient for Elasticsearch REST API
            var baseAddress = "https://localhost";
            if (!string.IsNullOrWhiteSpace(settings.CloudId))
            {
                baseAddress = settings.CloudId.StartsWith("http") ? settings.CloudId : $"https://{settings.CloudId}";
            }

            services.AddSingleton(settings);
            services.AddHttpClient<Services.IElasticLogger, Services.ElasticLogger>(c =>
            {
                c.BaseAddress = new Uri(baseAddress);
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrWhiteSpace(settings.ApiKey))
                {
                    c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("ApiKey", settings.ApiKey);
                }
            });

            return services;
        }
    }
}
