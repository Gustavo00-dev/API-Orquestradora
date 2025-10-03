namespace APIFCG.Infra.LogAPI
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }

    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        private static string _correlationId;
        public string Get() => _correlationId;
        public void Set(string correlationId) => _correlationId = correlationId;
    }
}
