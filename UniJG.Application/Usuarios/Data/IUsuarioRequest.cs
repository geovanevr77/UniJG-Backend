namespace UniJG.Application.Usuarios.Data
{
    public interface IUsuarioRequest
    {
        public string Codigo { get; }
        public string Email { get; }
        public string Nome { get; }
        public Task<string> GetToken();
    }
}