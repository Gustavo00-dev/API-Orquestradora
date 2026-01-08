using System;

namespace APIFCG.Infra.Model
{
    public class JogoMessage
    {
        public int idJogo { get; set; }
        public string Nome { get; set; }
        public string NomeAbreviado { get; set; }
        public DateTime DataLancamento { get; set; }
        public decimal ValorVenda { get; set; }
        public string UsuarioResponsavelCadastro { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}