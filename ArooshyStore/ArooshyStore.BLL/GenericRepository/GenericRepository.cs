//using ArooshyStore.DAL.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace ArooshyStore.BLL.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        internal DbSet<T> dbSet;

        #region Sync
        public GenericRepository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            this.dbSet = _unitOfWork.Db.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.AsEnumerable();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> whereCondition)
        {
            return dbSet.Where(whereCondition).AsEnumerable();
        }

        public T GetById(int id)
        {
            var dbResult = dbSet.Find(id);
            return dbResult;
        }

        public T GetByCondition(Expression<Func<T, bool>> whereCondition)
        {
            var dbResult = dbSet.Where(whereCondition).FirstOrDefault();
            return dbResult;
        }

        public T Insert(T entity)
        {

            var dbResult = dbSet.Add(entity);
            this._unitOfWork.Db.SaveChanges();
            return dbResult;

        }

        public T Update(T entity)
        {
            dbSet.Attach(entity);
            _unitOfWork.Db.Entry(entity).State = EntityState.Modified;
            this._unitOfWork.Db.SaveChanges();
            return entity;
        }

        public virtual void UpdateAll(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                dbSet.Attach(entity);
                _unitOfWork.Db.Entry(entity).State = EntityState.Modified;
            }
            this._unitOfWork.Db.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var dbResult = dbSet.Find(id);
            dbSet.Remove(dbResult);
            this._unitOfWork.Db.SaveChanges();
        }

        public void DeleteByCondition(Expression<Func<T, bool>> whereCondition)
        {
            IEnumerable<T> entities = this.GetAll(whereCondition);
            foreach (T entity in entities)
            {
                if (_unitOfWork.Db.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                dbSet.Remove(entity);
            }
            this._unitOfWork.Db.SaveChanges();
        }

        //--------------Exra generic methods--------------------------------

        public T SingleOrDefaultOrderBy(Expression<Func<T, bool>> whereCondition, Expression<Func<T, int>> orderBy, string direction)
        {
            if (direction == "ASC")
            {
                return dbSet.Where(whereCondition).OrderBy(orderBy).FirstOrDefault();

            }
            else
            {
                return dbSet.Where(whereCondition).OrderByDescending(orderBy).FirstOrDefault();
            }
        }

        public bool Exists(Expression<Func<T, bool>> whereCondition)
        {
            return dbSet.Any(whereCondition);
        }

        public int Count(Expression<Func<T, bool>> whereCondition)
        {
            return dbSet.Where(whereCondition).Count();
        }

        public IEnumerable<T> GetPagedRecords(Expression<Func<T, bool>> whereCondition, Expression<Func<T, string>> orderBy, int pageNo, int pageSize)
        {
            return (dbSet.Where(whereCondition).OrderBy(orderBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).AsEnumerable();
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters);
        }
        #endregion
        #region Async
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await dbSet.Where(whereCondition).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var dbResult = await dbSet.FindAsync(id);
            return dbResult;
        }

        public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> whereCondition)
        {
            var dbResult = await dbSet.Where(whereCondition).FirstOrDefaultAsync();
            return dbResult;
        }

        public async Task<T> InsertAsync(T entity)
        {

            var dbResult = dbSet.Add(entity);
            await this._unitOfWork.Db.SaveChangesAsync();
            return dbResult;

        }

        public async Task<T> UpdateAsync(T entity)
        {
            dbSet.Attach(entity);
            _unitOfWork.Db.Entry(entity).State = EntityState.Modified;
            await this._unitOfWork.Db.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteByIdAsync(T Entity)
        {
            string message = "";
            try
            {
                dbSet.Remove(Entity);
                await this._unitOfWork.Db.SaveChangesAsync();
                message = "Success";
            }
            catch (Exception ex) { message = ex.Message.ToString(); }
            return message;
        }


        //--------------Exra generic methods--------------------------------

        public async Task<T> SingleOrDefaultOrderByAsync(Expression<Func<T, bool>> whereCondition, Expression<Func<T, int>> orderBy, string direction)
        {
            if (direction == "ASC")
            {
                return await dbSet.Where(whereCondition).OrderBy(orderBy).FirstOrDefaultAsync();

            }
            else
            {
                return await dbSet.Where(whereCondition).OrderByDescending(orderBy).FirstOrDefaultAsync();
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await dbSet.AnyAsync(whereCondition);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await dbSet.Where(whereCondition).CountAsync();
        }

        public async Task<IEnumerable<T>> GetPagedRecordsAsync(Expression<Func<T, bool>> whereCondition, Expression<Func<T, string>> orderBy, int pageNo, int pageSize)
        {
            return await (dbSet.Where(whereCondition).OrderBy(orderBy).Skip((pageNo - 1) * pageSize).Take(pageSize)).ToListAsync();
        }
        #endregion
    }
}
