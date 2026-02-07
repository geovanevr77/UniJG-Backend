using UniJG.Application.Abstractions.Data;
using UniJG.Application.Abstractions.Interfaces;
using UniJG.Application.Autenticacoes.Data;

namespace UniJG.Domain.Services.Autenticacoes
{
    internal class TokenDeAcesso : ITokenDeAcesso
    {
        internal readonly IKeyCloakClient keyCloakClient;

        public TokenDeAcesso(IKeyCloakClient keyCloakClient)
        {
            this.keyCloakClient = keyCloakClient;
        }

        internal virtual string Token { get; set; }

        public async Task<Response<string>> ObterAsync()
        {
            if (Token is null)
            {
                List<KeyValuePair<string, string>> collection = new()
            {
                new("grant_type", "client_credentials"),
                new("client_id", "sicof-backend"),
                new("client_secret", Environment.GetEnvironmentVariable("SICOF_AUTH_SECRET_BACKEND"))
            };

                string endpoint = "protocol/openid-connect/token";
                Response<TokenDeAcessoResponse> response =
                    await keyCloakClient.PostAsync<TokenDeAcessoResponse>(endpoint, collection);

                Token = response.Result.Access_token;

                return new(Token);
            }

            return new(Token);
        }
    }
}