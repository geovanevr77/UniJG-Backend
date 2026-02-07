using System.Data;

namespace UniJG.Domain.Model.UnitOfWork.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}