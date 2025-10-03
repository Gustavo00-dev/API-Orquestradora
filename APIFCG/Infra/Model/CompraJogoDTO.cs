namespace APIFCG.Infra.Model
{
    /// <summary>
    /// Data Transfer Object (DTO) para representar uma compra de jogo.
    /// </summary>
    public class CompraJogoDTO
    {
        public int IdCliente { get; set; }
        public int IdJogo { get; set; }
        public DateTime DataCompra { get; set; }
        public decimal ValorCompra { get; set; }
        // Propriedades adicionais para facilitar a visualização
        public string NomeCliente { get; set; }
        public string NomeJogo { get; set; }
    }
}
