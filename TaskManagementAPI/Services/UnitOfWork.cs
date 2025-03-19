using Microsoft.EntityFrameworkCore.Storage;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interfaces;

namespace TaskManagementAPI.Services
{
    public class UnitOfWork(TaskManagementDbContext context) : IUnitOfWork
    {
        private readonly TaskManagementDbContext _context = context;
        private IDbContextTransaction _transaction;

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
