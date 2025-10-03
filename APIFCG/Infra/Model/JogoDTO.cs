namespace APIFCG.Infra.Model
{
    /// <summary>
    /// Data Transfer Object (DTO) para representar o cadastro de um jogo.
    /// </summary>
    public class JogoDTO
    {
        public string Nome { get; set; }
        public string NomeAbreviado { get; set; }
        public DateTime DataLancamento { get; set; }
        public decimal ValorVenda { get; set; }
        public string UsuarioResponsavelCadastro { get; set; }
    }
}
