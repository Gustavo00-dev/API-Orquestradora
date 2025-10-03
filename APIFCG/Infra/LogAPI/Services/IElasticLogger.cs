using System.Collections.Generic;
using System.Threading.Tasks;
using APIFCG.Infra.LogAPI.Models;

namespace APIFCG.Infra.LogAPI.Services
{
    public interface IElasticLogger
    {
        Task RegisterLogAsync(LogEntry entry, string index);
        Task<IEnumerable<LogEntry>> SearchLogsAsync(string query, string indexName, int size = 50);
    }
}
