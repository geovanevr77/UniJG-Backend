using UniJG.Application.Abstractions.Data;

namespace UniJG.Application.Abstractions.Interfaces
{
    public interface ITokenDeAcesso
    {
        Task<Response<string>> ObterAsync();
    }
}