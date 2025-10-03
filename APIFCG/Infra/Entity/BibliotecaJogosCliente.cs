using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFCG.Infra.Entity
{
    public class BibliotecaJogosCliente
    {
        [Key, Column(Order = 0)]
        public int IdUsuario { get; set; }

        [Key, Column(Order = 1)]
        public int IdJogo { get; set; }

        public DateTime DataCompra { get; set; }
        public decimal ValorCompra { get; set; }

        // Propriedade de navegação para Usuario
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; }

        // Propriedade de navegação para Jogo
        [ForeignKey(nameof(IdJogo))]
        public Jogo Jogo { get; set; }
    }
}
