using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIFCG.Infra.Entity
{
    public class PromocaoJogo
    {
        [Key, Column(Order = 0)]
        public int IdJogo { get; set; }

        public DateTime DataInicioPromocao { get; set; }
        public DateTime DataFimPromocao { get; set; }
        public decimal ValorJogoPromocao { get; set; }

        // Propriedade de navegação para Jogo
        [ForeignKey(nameof(IdJogo))]
        public Jogo Jogo { get; set; }
    }
}
