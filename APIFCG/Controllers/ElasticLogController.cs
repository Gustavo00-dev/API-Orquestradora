using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APIFCG.Infra.LogAPI.Models;
using APIFCG.Infra.LogAPI.Services;

namespace APIFCG.Controllers
{
    [ApiController]
    [Route("logs")]
    public class ElasticLogController : ControllerBase
    {
        private readonly IElasticLogger _elasticLogger;

        public ElasticLogController(IElasticLogger elasticLogger)
        {
            _elasticLogger = elasticLogger;
        }

        [HttpPost("postLog")]
        public async Task<IActionResult> PostLog([FromBody] LogEntry entry, [FromQuery] string indexName)
        {
            if (entry == null) return BadRequest();
            await _elasticLogger.RegisterLogAsync(entry, indexName);
            return Accepted();
        }

        [HttpGet("getLog")]
        public async Task<IActionResult> GetLog([FromQuery] string query, [FromQuery] string indexName, [FromQuery] int size = 50)
        {
            var result = await _elasticLogger.SearchLogsAsync(query, indexName, size);
            return Ok(result);
        }
    }
}
