using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UniJG.Application.Abstractions.Helpers;
using UniJG.Domain.Model.UnitOfWork.Enum;
using UniJG.Domain.Model.UnitOfWork.Interface;

namespace UniJG.Infrastructure.Repositories.Context
{
    internal class UniDbContext : DbContext, IUnitOfWork
    {
        public UniDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniDbContext).Assembly);

        void IUnitOfWork.Add(object entity)
            => Add(entity);

        void IUnitOfWork.Remove(object entity)
        {
            base.Remove(entity);
        }

        public async Task AddAsync(object entity)
            => await base.AddAsync(entity);

        async Task<StatusCommit> IUnitOfWork.Commit(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                return StatusCommit.Sucesso;
            } catch (DbUpdateConcurrencyException ex)
            {
                ElasticLogHelper.LogJsonContent("DbUpdateConcurrencyException", SerializeObject(ex));
                return StatusCommit.Concorrencia;
            } catch (DbUpdateException ex)
            {
                ElasticLogHelper.LogJsonContent("DbUpdateException", SerializeObject(ex));
                return StatusCommit.Falha;
            }
        }

        private static string SerializeObject(Exception exception)
            => JsonConvert.SerializeObject(exception, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 6,
                Formatting = Formatting.None
            });
    }
}