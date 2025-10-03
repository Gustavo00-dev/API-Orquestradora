namespace APIFCG.Infra.Model
{
    /// <summary>
    /// Data Transfer Object (DTO) para representar uma promoção de jogo.
    /// </summary>
    public class PromocaoJogoDTO
    {
        public int IdUsuario { get; set; }
        public int IdJogo { get; set; }
        public DateTime DataInicioPromocao { get; set; }
        public DateTime DataFimPromocao { get; set; }
        public decimal ValorJogoPromocao { get; set; }
    }
}
