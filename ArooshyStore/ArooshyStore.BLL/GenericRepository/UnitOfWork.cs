using System.Data.Entity;
using ArooshyStore.DAL.Data;

namespace ArooshyStore.BLL.GenericRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ModelDbContext _dbContext;
        //Here we define Edmx object in constructor of UnitOfWork
        public UnitOfWork()
        {
            _dbContext = new ModelDbContext();
        }

        //Here we assign _dbContext to Db so we can access everyting in Db property
        public DbContext Db
        {
            get { return _dbContext; }
        }

        //As IUnitOfWork inhreit from IDisposible, so we need to implement method of IDisposible but here nothing to do with it
        public void Dispose()
        {
        }
    }
}
