using Newtonsoft.Json;
using UniJG.Application.Abstractions.Data;
using UniJG.Application.Abstractions.Helpers;
using UniJG.Application.Abstractions.Interfaces;

namespace UniJG.Domain.Services.Clients
{
    public class KeyCloakClient : IKeyCloakClient
    {
        internal readonly HttpClient httpClient;

        public KeyCloakClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Response<TResult>> PostAsync<TResult>(string endpoint, List<KeyValuePair<string, string>> collection)
        {
            try
            {
                HttpRequestMessage request = new(HttpMethod.Post, endpoint);
                FormUrlEncodedContent content = new(collection);
                request.Content = content;
                using HttpResponseMessage result = await httpClient.SendAsync(request);

                ResponseStatus responseStatus = (ResponseStatus)result.StatusCode;

                if (result.IsSuccessStatusCode)
                {
                    return MapearStringParaResponse<TResult>(await result.Content.ReadAsStringAsync());
                }

                return new(
                    responseStatus: responseStatus,
                    message: $"Não foi possível processar a resposta da integração com o KeyCloak (Status: {(int)responseStatus}).");
            } catch (Exception exception)
            {
                return new(exception);
            }
        }

        internal static Response<TResult> MapearStringParaResponse<TResult>(string content)
        {
            try
            {
                TResult result = JsonConvert.DeserializeObject<TResult>(content);

                return new(result);
            } catch (Exception)
            {
                ElasticLogHelper.LogJsonContent("JSON Response", content);
                return new Response<TResult>(
                    responseStatus: ResponseStatus.InternalServerError,
                    message: $"Não foi possível processar a resposta da integração com o KeyCloak (Status {(int)ResponseStatus.InternalServerError}).");
            }
        }
    }
}