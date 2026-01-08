using System.Threading.Tasks;
using APIFCG.Infra.Model;

namespace APIFCG.Infra.Services
{
    public interface IJogoProducer
    {
        Task EnqueueCadastrarJogoAsync(JogoMessage jogoMessage);
    }
}