namespace APIFCG.Infra.Model
{
    /// <summary>
    /// DTO para autenticação de usuários.
    /// </summary>
    public class LoginDTO
    {
        /// <summary> Email de usuário para autenticação.  </summary>
        public string Email { get; set; }
        /// <summary> Senha do usuário para autenticação. </summary>
        public string Password { get; set; }
    }
}
