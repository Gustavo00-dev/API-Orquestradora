using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using APIFCG.Infra.LogAPI.Models;

namespace APIFCG.Infra.LogAPI.Services
{
    public class ElasticLogger : IElasticLogger
    {
        private readonly HttpClient _http;
        private readonly ElasticSettings _settings;

        public ElasticLogger(HttpClient http, ElasticSettings settings)
        {
            _http = http;
            _settings = settings;
            if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", _settings.ApiKey);
            }
        }

        public async Task RegisterLogAsync(LogEntry entry, string index)
        {
            if (entry == null) return;
            var json = JsonSerializer.Serialize(entry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _http.PostAsync($"{index}/_doc", content);
        }

        public async Task<IEnumerable<LogEntry>> SearchLogsAsync(string query, string indexName, int size = 50)
        {
            var body = new
            {
                size,
                query = new
                {
                    simple_query_string = new
                    {
                        query = string.IsNullOrWhiteSpace(query) ? "*" : query,
                        fields = new[] { "Message", "Level", "CorrelationId" },
                        default_operator = "and"
                    }
                },
                sort = new[] { new { Timestamp = new { order = "desc" } } }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync($"{indexName}/_search", content);
            if (!resp.IsSuccessStatusCode) return Enumerable.Empty<LogEntry>();

            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var hits = doc.RootElement.GetProperty("hits").GetProperty("hits");
            var list = new List<LogEntry>();
            foreach (var h in hits.EnumerateArray())
            {
                if (h.TryGetProperty("_source", out var src))
                {
                    try
                    {
                        var entry = JsonSerializer.Deserialize<LogEntry>(src.GetRawText());
                        if (entry != null) list.Add(entry);
                    }
                    catch { }
                }
            }

            return list;
        }
    }
}
