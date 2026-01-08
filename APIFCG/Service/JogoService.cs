
using APIFCG.Infra.Entity;
using APIFCG.Infra.LogAPI;
using APIFCG.Infra.Model;
using Microsoft.AspNetCore.Mvc;

namespace APIFCG.Service
{
    public interface IJogoService
    {
        List<Jogo> ObterTodosJogos();
        List<Jogo> ObterJogosPorCliente(int idtCliente);
        IActionResult CadastrarJogo(Jogo jogo);
        Jogo ObterJogoPorId(int idtJogo);
        IActionResult CadastrarPromocao(PromocaoJogo promocao);
        IActionResult ComprarJogo(int idUsuario, int idJogo);
    }
    public class JogoService : IJogoService
    {
        private readonly BaseLogger<JogoService> _logger;
        public JogoService(BaseLogger<JogoService> logger)
        {
            _logger = logger;
        }

        public List<Jogo> ObterTodosJogos()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "69a694132506419fba55c3802ac36aad");

                var response = httpClient.GetAsync("https://gerencimentoapi.azure-api.net/msjogos/api/Jogo/BuscarTodosJogos").Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;
                var jogos = System.Text.Json.JsonSerializer.Deserialize<List<Jogo>>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return jogos ?? new List<Jogo>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao obter jogos da API: " + ex.Message, "vendas");
                return new List<Jogo>();
            }
        }
        public List<Jogo> ObterJogosPorCliente(int idtCliente)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "69a694132506419fba55c3802ac36aad");

                var response = httpClient.GetAsync($"https://gerencimentoapi.azure-api.net/msjogos/api/Jogo/ListarJogosPorId?idtCliente={idtCliente}").Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;
                var jogos = System.Text.Json.JsonSerializer.Deserialize<List<Jogo>>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return jogos ?? new List<Jogo>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao obter jogos do cliente da API: " + ex.Message, "jogos");
                return new List<Jogo>();
            }
        }
        public IActionResult CadastrarJogo(Jogo jogo)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "69a694132506419fba55c3802ac36aad");

                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(jogo), System.Text.Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("https://gerencimentoapi.azure-api.net/msjogos/api/Jogo/CadastrarNovoJogo", jsonContent).Result;
                response.EnsureSuccessStatusCode();

                return new OkObjectResult("Jogo cadastrado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao cadastrar jogo na API: " + ex.Message, "jogos");
                return new BadRequestObjectResult("Erro ao cadastrar jogo: " + ex.Message);
            }
        }
        public Jogo ObterJogoPorId(int idtJogo)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "69a694132506419fba55c3802ac36aad");

                var response = httpClient.GetAsync($"https://gerencimentoapi.azure-api.net/msjogos/api/Jogo/ListarJogosPorId?idtJogo={idtJogo}").Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;
                var jogo = System.Text.Json.JsonSerializer.Deserialize<Jogo>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            
                return jogo ?? new Jogo();
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao obter jogo por ID da API: " + ex.Message , "jogos");
                return new Jogo();
            }
        }
        public IActionResult CadastrarPromocao(PromocaoJogo promocao)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "69a694132506419fba55c3802ac36aad");

                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(promocao), System.Text.Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("https://gerencimentoapi.azure-api.net/msjogos/api/Jogo/CadastrarPromocao", jsonContent).Result;
                response.EnsureSuccessStatusCode();
                _logger.LogInformation($"Promoção cadastrada com sucesso para o jogo {promocao.IdJogo}.", "jogos");
                return new OkObjectResult("Promoção cadastrada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao cadastrar promoção na API: " + ex.Message, "jogos");
                return new BadRequestObjectResult("Erro ao cadastrar promoção: " + ex.Message);
            }
        }
        public IActionResult ComprarJogo(int idUsuario, int idJogo)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "69a694132506419fba55c3802ac36aad");

                var compraData = new { IdUsuario = idUsuario, IdJogo = idJogo };
                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(compraData), System.Text.Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("https://gerencimentoapi.azure-api.net/msjogos/api/Jogo/ComprarJogo?IdUsuario=" + idUsuario + "&IdJogo=" + idJogo, jsonContent).Result;
                
                response.EnsureSuccessStatusCode();
                _logger.LogInformation($"Jogo {idJogo} comprado com sucesso por usuário {idUsuario}.", "vendas");
                return new OkObjectResult("Jogo comprado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao comprar jogo na API: " + ex.Message, "vendas");
                return new BadRequestObjectResult("Erro ao comprar jogo: " + ex.Message);
            }
        }
    }
}
