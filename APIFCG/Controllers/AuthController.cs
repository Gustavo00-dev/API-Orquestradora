using APIFCG.Infra.LogAPI;
using APIFCG.Infra.Model;
using APIFCG.Service;
using Microsoft.AspNetCore.Mvc;

namespace APIFCG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUsuarioService _usuarioService;
        private readonly BaseLogger<AuthController> _logger;

        public AuthController(
            IJwtService jwtService,
            IUsuarioService usuarioService,
            BaseLogger<AuthController> logger
        )
        {
            _jwtService = jwtService;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoit para autenticação de usuários.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>Token</returns>
        [HttpPost("loginAuth")]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            try
            {
                _logger.LogInformation($"Tentativa de login de usuario. {login.Email}", "login-auth");
                if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                {
                    _logger.LogWarning("Tentativa de login inválida: nome de usuário ou senha invalidos.", "login-auth");
                    return BadRequest("Tentativa de Login Invalida.");
                }

                var usuario = _usuarioService.ObterTodos()
                    .FirstOrDefault(u => u.Email == login.Email && u.Senha == login.Password);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuario ou senha não existe.", "login-auth");
                    return Unauthorized("Senha ou Usuario invalido.");
                }
                // Define o papel do usuário com base no nível de acesso - 1 Usuario comum, 2 Admin.
                var role = usuario.Lvl == 2 ? "Admin" : "User";
                var token = _jwtService.GenerateToken(login.Email, role);
                _logger.LogInformation($"Login realizado com sucesso para o usuario [{login.Email}].", "login-auth");
                return Ok(new { token });
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao realizar login: {e.Message}", "login-auth");
                return Unauthorized();
            }
        }
    }
}
