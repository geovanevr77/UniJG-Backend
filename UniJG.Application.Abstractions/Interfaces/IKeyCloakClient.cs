using UniJG.Application.Abstractions.Data;

namespace UniJG.Application.Abstractions.Interfaces
{
    public interface IKeyCloakClient
    {
        Task<Response<TResult>> PostAsync<TResult>(
            string endpoint,
            List<KeyValuePair<string, string>> collection);
    }
}