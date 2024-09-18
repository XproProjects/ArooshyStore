using System;
using System.Data.Entity;

namespace ArooshyStore.BLL.GenericRepository
{
    public interface IUnitOfWork : IDisposable
    {
        //IDisposable will free up objects from memory which are not in used and will destroy its object.
        /// <summary>
        /// Return the database reference for this UOW
        /// </summary>
        DbContext Db { get; }
    }
}
