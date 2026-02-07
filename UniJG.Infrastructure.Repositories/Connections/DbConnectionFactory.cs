using Microsoft.Data.SqlClient;
using System.Data;
using UniJG.Domain.Model.UnitOfWork.Interface;

namespace UniJG.Infrastructure.Repositories.Connections
{
    internal class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string connectionString;

        public DbConnectionFactory(
            string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            Elastic.Apm.Api.ISpan span = Elastic.Apm.Agent.Tracer.CurrentTransaction?.StartSpan("connection", "db", "sql server");

            try
            {
                SqlConnection connection = new(connectionString);
                connection.Open();
                return connection;
            } catch (Exception e)
            {
                span?.CaptureException(e);
                throw;
            } finally
            {
                span?.End();
            }
        }
    }
}