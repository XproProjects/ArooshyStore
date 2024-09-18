using System;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;

namespace ArooshyStore.BLL.Services
{
    public class HomeRepository : IHomeRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //public void Dispose()
        //{
        //}
    }
}
