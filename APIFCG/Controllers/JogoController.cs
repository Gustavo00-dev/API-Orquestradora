using APIFCG.Infra.LogAPI;
using APIFCG.Infra.Model;
using APIFCG.Infra.Services;
using APIFCG.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIFCG.Controllers
{
    /// <summary>
    /// Controller para gerenciar jogos.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JogoController : ControllerBase
    {
        private readonly BaseLogger<UsuarioController> _logger;
        private readonly IJogoService _jogoService;
        private readonly IJogoProducer _jogoProducer;


        public JogoController(
            BaseLogger<UsuarioController> logger,
            IJogoService jogoService,
            IJogoProducer jogoProducer
        )
        {
            _logger = logger;
            _jogoService = jogoService;
            _jogoProducer = jogoProducer;
        }

        /// <summary>
        /// Listar todos os jogos.
        /// </summary>
        /// <returns>Todos os jogos disponiveis</returns>
        [HttpGet("BuscarTodosJogos")]
        public IActionResult GetTodosJogos()
        {
            try
            {
                _logger.LogInformation("Listar todos jogos.", "Jogos");
                var jogos = _jogoService.ObterTodosJogos();
                return Ok(jogos);
            }
            catch (Exception e)
            {
                return BadRequest($"Erro ao listar todos os jogos: {e.Message}");
            }
        }

        /// <summary>
        /// Listar todos os jogos de um cliente específico.
        /// </summary>
        /// <param name="idtCliente"></param>
        /// <returns></returns>
        [HttpGet("ListarTodosJogosCliente")]
        public IActionResult ListarJogosCliente(int idtCliente)
        {
            try
            {
                _logger.LogInformation($"Listando jogos do cliente {idtCliente}.","Jogos");
                var jogos = _jogoService.ObterJogosPorCliente(idtCliente);
                if (jogos == null || !jogos.Any())
                {
                    _logger.LogWarning($"Nenhum jogo encontrado para o cliente {idtCliente}.","Jogos");
                    return NotFound("Nenhum jogo encontrado para este cliente.");
                }
                return Ok(jogos);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao listar jogos do cliente: {e.Message}", "Jogos");
                return BadRequest($"Erro ao listar jogos do cliente: {e.Message}");
            }
        }
        /// <summary>
        /// Endpoint para cadastrar um novo jogo.
        /// </summary>
        /// <param name="jogoDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("CadastrarNovoJogo")]
        public async Task<IActionResult> CadastrarJogo(JogoDTO jogoDTO)
        {
            try
            {
                _logger.LogInformation("Cadastrando novo jogo.", "Jogos");
                if (string.IsNullOrEmpty(jogoDTO.Nome) || jogoDTO.ValorVenda <= 0)
                {
                    _logger.LogWarning("Tentativa de cadastro de jogo inválida: dados incompletos.", "Jogos");
                    return BadRequest("Dados inválidos para cadastro de jogo.");
                }

                var jogo = new Infra.Entity.Jogo
                {
                    Nome = jogoDTO.Nome,
                    NomeAbreviado = jogoDTO.NomeAbreviado,
                    DataLancamento = jogoDTO.DataLancamento,
                    ValorVenda = jogoDTO.ValorVenda,
                    UsuarioResponsavelCadastro = jogoDTO.UsuarioResponsavelCadastro ?? "Admin", // Defult para Admin
                };

                // Enfileira mensagem no Service Bus sobre o novo cadastro
                var jogoMessage = new APIFCG.Infra.Model.JogoMessage
                {
                    idJogo = jogo.idJogo,
                    Nome = jogo.Nome,
                    NomeAbreviado = jogo.NomeAbreviado,
                    DataLancamento = jogo.DataLancamento,
                    ValorVenda = jogo.ValorVenda,
                    UsuarioResponsavelCadastro = jogo.UsuarioResponsavelCadastro,
                    DataCadastro = DateTime.UtcNow
                };

                try
                {
                    await _jogoProducer.EnqueueCadastrarJogoAsync(jogoMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao enfileirar mensagem no Service Bus: {ex.Message}", "ServiceBus");
                    // não falha o cadastro por conta de erro na fila
                }

                _logger.LogInformation($"Jogo {jogo.Nome} cadastrado com sucesso.", "Jogos");
                return StatusCode(200, "Jogo Cadastrado com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao cadastrar jogo: {e.Message}", "Jogos");
                return BadRequest($"Erro ao cadastrar jogo: {e.Message}");
            }
        }

        /// <summary>
        /// Endpoint para cadastrar uma nova promoção de jogo.
        /// </summary>
        /// <param name="promocaoDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("CadastrarPromoçoes")]
        public IActionResult CadastrarPromocao(PromocaoJogoDTO promocaoDTO)
        {
            try
            {
                _logger.LogInformation("Cadastrando promoção de jogo.", "Jogos");
                if (promocaoDTO.IdJogo <= 0)
                {
                    _logger.LogWarning("Tentativa de cadastro de promoção inválida: ID do jogo inválido.", "Jogos");
                    return BadRequest("Dados inválidos para cadastro de promoção.");
                }
               
                // Cadastra a nova promoção
                var promocao = new Infra.Entity.PromocaoJogo
                {
                    IdJogo = promocaoDTO.IdJogo,
                    DataInicioPromocao = promocaoDTO.DataInicioPromocao,
                    DataFimPromocao = promocaoDTO.DataFimPromocao,
                    ValorJogoPromocao = promocaoDTO.ValorJogoPromocao
                };
                
                var response = _jogoService.CadastrarPromocao(promocao);
                if (response is BadRequestObjectResult)
                {
                    _logger.LogWarning("Falha ao cadastrar promoção via serviço.", "Jogos");
                    return response;
                }

                _logger.LogInformation($"Promoção cadastrada com sucesso para o jogo {promocao.IdJogo}.", "Jogos");
                return StatusCode(201, $"Promoção cadastrada com sucesso para o jogo {promocao.IdJogo}.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao cadastrar promoção: {e.Message}", "Jogos");
                return BadRequest($"Erro ao cadastrar promoção: {e.Message}");
            }
        }

        /// <summary>
        /// Endpoint para comprar um jogo.
        /// </summary>
        /// <returns>Status da Compra</returns>
        [HttpPost("ComprarJogo")]
        public IActionResult ComprarJogo(int idUsuario, int idJogo)
        {
            try
            {
                _logger.LogInformation("Comprando jogo.", "vendas");
                if (idUsuario <= 0 || idJogo <= 0)
                {
                    _logger.LogWarning("Tentativa de compra de jogo inválida: ID do usuário ou do jogo inválido.", "vendas");
                    return BadRequest("Dados inválidos para compra de jogo.");
                }

                
                var reponse = _jogoService.ComprarJogo(idUsuario, idJogo);
                if (reponse is BadRequestObjectResult)
                {
                    _logger.LogWarning("Falha ao comprar jogo via serviço.", "vendas");
                    return reponse;
                }
                _logger.LogInformation($"Jogo {idJogo} comprado com sucesso por usuário {idUsuario}.", "vendas");
                return StatusCode(200, $"Jogo {idJogo} comprado com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao comprar jogo: {e.Message}", "vendas");
                return BadRequest($"Erro ao comprar jogo: {e.Message}");
            }
        }
    }
}
