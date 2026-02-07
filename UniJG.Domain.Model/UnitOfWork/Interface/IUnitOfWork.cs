using UniJG.Domain.Model.UnitOfWork.Enum;

namespace UniJG.Domain.Model.UnitOfWork.Interface
{
    public interface IUnitOfWork
    {
        void Add(object entity);

        Task AddAsync(object entity);

        void Remove(object entity);

        Task<StatusCommit> Commit(CancellationToken cancellationToken);
    }
}