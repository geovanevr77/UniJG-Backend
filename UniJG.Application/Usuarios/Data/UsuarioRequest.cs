using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace UniJG.Application.Usuarios.Data
{
    public class UsuarioRequest : IUsuarioRequest
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public const string PreferredUsername = "preferred_username";
        public const string Username = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string Name = "name";

        public UsuarioRequest(
            IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string Email
        {
            get => ObterEmail(_contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(u => u.Type.Equals(PreferredUsername))
                ?.Value);
        }

        public string Codigo
        {
            get => ObterCodigo(_contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(u => u.Type.Equals(PreferredUsername))
                ?.Value);
        }

        public string Nome
        {
            get => _contextAccessor.HttpContext.User.Claims
            .FirstOrDefault(u => u.Type.Equals(Username) || u.Type.Equals(Name))
            ?.Value;
        }

        internal static string ObterEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }
            return email.Contains('@') ? email : $"{email}@gmail.com";
        }

        internal static string ObterCodigo(string codigo)
        {
            return codigo?.Split('@')[0]?.ToUpper() ?? codigo?.ToUpper();
        }

        public async Task<string> GetToken()
        {
            if (_contextAccessor == null || _contextAccessor.HttpContext == null)
            {
                return string.Empty;
            }

            return await _contextAccessor.HttpContext.GetTokenAsync("access_token");
        }
    }
}