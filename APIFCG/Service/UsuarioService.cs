using APIFCG.Infra.Entity;
using APIFCG.Infra.LogAPI;
using APIFCG.Infra.Model;
using Microsoft.AspNetCore.Mvc;

namespace APIFCG.Service
{
    public interface IUsuarioService
    {
        UsuarioDTO CadastrarUsuario(UsuarioDTO usuario);
        List<Usuario> ObterTodos();
        IActionResult CadastrarNovoUsuario(Usuario usuario);
        Usuario ObterUsuarioPorId(int id);
        IActionResult AlterarNivelUsuario(Usuario usuario);
        IActionResult DeletarUsuario(Usuario usuario);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly BaseLogger<UsuarioService> _logger;
        public UsuarioService(
            BaseLogger<UsuarioService> logger
        )
        {
            _logger = logger;
        }

        public UsuarioDTO CadastrarUsuario(UsuarioDTO usuario)
        {
            _logger.LogInformation($"Cadastrando usuário: {usuario.Nome}", "usuarios");

            return usuario;
        }
        public List<Usuario> ObterTodos()
        {
            _logger.LogInformation("Obtendo todos os usuários.", "usuarios");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "abec3fafe03345c4bcbaf0d3ee965366");
                var response = httpClient.GetAsync("https://fiap-apigatw-arquitetura.azure-api.net/ms-usuarios/api/Base/GetTodosUsuarios").Result;

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    _logger.LogError($"Erro ao obter usuários: {response.StatusCode} - {errorContent}", "usuarios");
                    throw new Exception($"Erro ao obter usuários: {response.StatusCode} - {errorContent}");
                }

                var json = response.Content.ReadAsStringAsync().Result;
                var usuarios = System.Text.Json.JsonSerializer.Deserialize<List<Usuario>>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return usuarios ?? new List<Usuario>();
            }
        }
        public IActionResult CadastrarNovoUsuario(Usuario usuario)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var usuarioPayload = new
                    {
                        nome = usuario.Nome,
                        email = usuario.Email,
                        senha = usuario.Senha
                    };

                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "abec3fafe03345c4bcbaf0d3ee965366");
                    var response = httpClient.PostAsJsonAsync(
                        "https://fiap-apigatw-arquitetura.azure-api.net/ms-usuarios/api/Base/CadastrarNovoUsuario",
                        usuarioPayload
                    ).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.Content.ReadAsStringAsync().Result;
                        throw new Exception($"Erro ao chamar o endpoint: {response.StatusCode} - {errorContent}");
                    }
                }

                // Simulação de cadastro - em um cenário real, você salvaria esses dados em um banco de dados.
                _logger.LogInformation($"Usuário {usuario.Nome} cadastrado com sucesso.", "usuarios");

                return new OkObjectResult($"Usuário {usuario.Nome} cadastrado com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao cadastrar usuário: {e.Message}", "usuarios");
                return new BadRequestObjectResult($"Erro ao cadastrar usuário: {e.Message}");
            }
        }
        public Usuario ObterUsuarioPorId(int id)
        {
            _logger.LogInformation($"Obtendo usuário com ID: {id}", "usuarios");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "abec3fafe03345c4bcbaf0d3ee965366");
                var response = httpClient.GetAsync($"https://fiap-apigatw-arquitetura.azure-api.net/ms-usuarios/api/Base/GetUsuarioPorId?id={id}").Result;
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    _logger.LogError($"Erro ao obter usuário por ID: {response.StatusCode} - {errorContent}", "usuarios");
                    throw new Exception($"Erro ao obter usuário por ID: {response.StatusCode} - {errorContent}");
                }

                var json = response.Content.ReadAsStringAsync().Result;
                var usuarios = System.Text.Json.JsonSerializer.Deserialize<Usuario>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return usuarios ?? new Usuario();
            }
        }

        public IActionResult AlterarNivelUsuario(Usuario usuario)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var usuarioPayload = new
                    {
                        idUsuario = usuario.IdUsuario,
                        novoLvl = usuario.Lvl
                    };

                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "abec3fafe03345c4bcbaf0d3ee965366");

                    //O endpoint espera o idUsuario na query e o novoLvl no body
                    var url = $"https://fiap-apigatw-arquitetura.azure-api.net/ms-usuarios/api/Base/AlterarNivelUsuario?idUsuario={usuario.IdUsuario}&novoLvl={usuario.Lvl}";
                    var response = httpClient.PutAsJsonAsync(url, usuarioPayload).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.Content.ReadAsStringAsync().Result;
                        throw new Exception($"Erro ao chamar o endpoint: {response.StatusCode} - {errorContent}");
                    }
                }

                _logger.LogInformation($"Nível do usuário {usuario.IdUsuario} alterado para {usuario.Lvl} com sucesso.", "usuarios");

                return new OkObjectResult($"Nível do usuário {usuario.IdUsuario} alterado para {usuario.Lvl} com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao alterar nível do usuário: {e.Message}", "usuarios");
                return new BadRequestObjectResult($"Erro ao alterar nível do usuário: {e.Message}");
            }
        }
        public IActionResult DeletarUsuario(Usuario usuario)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "abec3fafe03345c4bcbaf0d3ee965366");
                    var response = httpClient.DeleteAsync($"https://fiap-apigatw-arquitetura.azure-api.net/ms-usuarios/api/Base/DeletarUsuario?idUsuarioDeletar={usuario.IdUsuario}").Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = response.Content.ReadAsStringAsync().Result;
                        throw new Exception($"Erro ao chamar o endpoint: {response.StatusCode} - {errorContent}");
                    }
                }

                _logger.LogInformation($"Usuário com ID {usuario.IdUsuario} deletado com sucesso.", "usuarios");

                return new OkObjectResult($"Usuário com ID {usuario.IdUsuario} deletado com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao deletar usuário: {e.Message}", "usuarios");
                return new BadRequestObjectResult($"Erro ao deletar usuário: {e.Message}");
            }
        }
    }
}
