using Microsoft.EntityFrameworkCore;

namespace Proyecto_alcaldia.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    DbContext Context { get; }
    IODSRepository ODS { get; }
    Task<int> SaveChangesAsync();
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
