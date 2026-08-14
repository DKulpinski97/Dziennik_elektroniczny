using Dziennik_szkolny.Application.Interfejsy;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dziennik_szkolny.Infrastructure.Serwisy
{
    public class JednostkaPracy : IJednostkaPracy
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction? _transakcja;

        public JednostkaPracy(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RozpocznijTransakcjeAsync()
        {
            _transakcja =
                await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task ZatwierdzAsync()
        {
            await _dbContext.SaveChangesAsync();

            if (_transakcja != null)
            {
                await _transakcja.CommitAsync();
                await _transakcja.DisposeAsync();

                _transakcja = null;
            }
        }

        public async Task CofnijAsync()
        {
            if (_transakcja != null)
            {
                await _transakcja.RollbackAsync();
                await _transakcja.DisposeAsync();

                _transakcja = null;
            }
        }
    }
}
