using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ArooshyStore.BLL.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        #region Sync
        /// Returns all the rows for type T
        IEnumerable<T> GetAll();

        /// Returns all the rows for type T on basis of filter condition
        IEnumerable<T> GetAll(Expression<Func<T, bool>> whereCondition);

        /// Returns single record by id for type T
        T GetById(int id);

        /// Retrieve a single item by it's primary key or return null if not found
        T GetByCondition(Expression<Func<T, bool>> whereCondition);

        /// Inserts the data into the table
        T Insert(T entity);

        /// Updates this entity in the database using it's primary key
        T Update(T entity);


        /// Updates all the passed entities in the database 
        void UpdateAll(IList<T> entities);

        /// Deletes this entry from the database
        /// ** WARNING - Most items should be marked inactive and Updated, not deleted
        void DeleteById(int id);

        /// Deletes this entry from the database
        /// ** WARNING - Most items should be marked inactive and Updated, not deleted
        void DeleteByCondition(Expression<Func<T, bool>> whereCondition);

        /// Does this item exist by it's primary key
        bool Exists(Expression<Func<T, bool>> whereCondition);
        #endregion
        #region Async
        /// Returns all the rows for type T
        Task<IEnumerable<T>> GetAllAsync();

        /// Returns all the rows for type T on basis of filter condition
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition);

        /// Returns single record by id for type T
        Task<T> GetByIdAsync(int id);

        /// Retrieve a single item by it's primary key or return null if not found
        Task<T> GetByConditionAsync(Expression<Func<T, bool>> whereCondition);

        /// Inserts the data into the table
        Task<T> InsertAsync(T entity);

        /// Updates this entity in the database using it's primary key
        Task<T> UpdateAsync(T entity);


        /// Deletes this entry from the database
        /// ** WARNING - Most items should be marked inactive and Updated, not deleted
        Task<string> DeleteByIdAsync(T entity);

        /// Does this item exist by it's primary key
        Task<bool> ExistsAsync(Expression<Func<T, bool>> whereCondition);
        #endregion
    }
}
