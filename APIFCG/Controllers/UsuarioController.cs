using APIFCG.Infra.LogAPI;
using APIFCG.Infra.Model;
using APIFCG.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace APIFCG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly BaseLogger<UsuarioController> _logger;
        private readonly IUsuarioService _usuarioService;
        
        public UsuarioController(
            BaseLogger<UsuarioController> logger,
            IUsuarioService usuarioService
        )
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }
        
        /// <summary>
        /// Lista todos os usuários cadastrados no sistema.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetTodosUsuarios()
        {
            try
            {
                _logger.LogInformation("Listar todos usuarios.", "Usuarios");
                var usuarios = _usuarioService.ObterTodos();
                return Ok(usuarios);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao listar todos os usuarios: {e.Message}", "Usuarios");
                return BadRequest($"Erro ao listar todos os usuarios: {e.Message}");
            }
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>        
        [HttpPost("CadastrarNovoUsuario")]
        public IActionResult CreateUsuario(UsuarioDTO usuario)
        {
            try
            {
                _logger.LogInformation("Criando novo usuario.", "Usuarios");

                if (usuario == null || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Senha))
                {
                    _logger.LogWarning("Tentativa de criação de usuário inválida: nome, email ou senha inválidos.", "Usuarios");
                    return BadRequest("Dados inválidos para criação de usuário.");
                }

                if (_usuarioService.ObterTodos().Any(u => u.Email == usuario.Email))
                {
                    _logger.LogWarning($"Email {usuario.Email} já está cadastrado.", "Usuarios");
                    return BadRequest("Email já cadastrado.");
                }

                bool senhaValida = Regex.IsMatch(usuario.Senha, @"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$");
                if (!senhaValida)
                {
                    _logger.LogWarning("Senha deve conter pelo menos 8 caracteres, incluindo letras, números e símbolos.", "Usuarios");
                    return BadRequest("Senha deve conter pelo menos 8 caracteres, incluindo letras, números e símbolos.");
                }

                if (!usuario.Email.Contains("@") || !usuario.Email.Contains("."))
                {
                    _logger.LogWarning("Email inválido.", "Usuarios");
                    return BadRequest("Email inválido.");
                }

                var resultado = _usuarioService.CadastrarNovoUsuario(new Infra.Entity.Usuario
                {
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Senha = usuario.Senha,
                    Lvl = 1
                });
                if (resultado is BadRequestObjectResult badRequest)
                {
                    return BadRequest(badRequest.Value);
                }
                return StatusCode(200, "Usuario criado com sucesso.");

            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao criar usuario: {e.Message}", "Usuarios");
                return BadRequest($"Erro ao criar usuario: {e.Message}");
            }
        }

        /// <summary>
        /// Altera o nível (Lvl) de um usuário.
        /// </summary>
        /// <param name="idUsuario">ID do usuário</param>
        /// <param name="novoLvl">Novo nível a ser atribuído</param>
        /// <returns>Resultado da operação</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("AlterarNivelUsuario")]
        public IActionResult AlterarNivelUsuario(int idUsuario, int novoLvl)
        {
            try
            {
                _logger.LogInformation($"Alterando nível do usuário {idUsuario} para {novoLvl}.", "Usuarios");

                var usuario = _usuarioService.ObterUsuarioPorId(idUsuario);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuário com ID {idUsuario} não encontrado.", "Usuarios");
                    return NotFound($"Usuário com ID {idUsuario} não encontrado.");
                }

                usuario.Lvl = novoLvl;
                _usuarioService.AlterarNivelUsuario(usuario);

                return Ok($"Nível do usuário {idUsuario} alterado para {novoLvl} com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao alterar nível do usuário: {e.Message}", "Usuarios");
                return BadRequest($"Erro ao alterar nível do usuário: {e.Message}");
            }
        }

        /// <summary>
        /// Deleta um usuário pelo ID.
        /// </summary>
        /// <param name="idUsuarioDeletar"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletarUsuario")]
        public IActionResult DeletarUsuario(int idUsuarioDeletar)
        {
            try
            {
                _logger.LogInformation($"Tentando deletar usuário com ID {idUsuarioDeletar}.", "usuarios");

                var usuario = _usuarioService.ObterUsuarioPorId(idUsuarioDeletar);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuário com ID {idUsuarioDeletar} não encontrado.", "usuarios");
                    return NotFound($"Usuário com ID {idUsuarioDeletar} não encontrado.");
                }

                _usuarioService.DeletarUsuario(usuario);
                return Ok($"Usuário com ID {idUsuarioDeletar} deletado com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao deletar usuário: {e.Message}", "usuarios");
                return BadRequest($"Erro ao deletar usuário: {e.Message}");
            }
        }
    }
}
