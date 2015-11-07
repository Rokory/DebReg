using DebReg.Data;
using System;

namespace DebRegComponents
{
    public class BaseManager
    {
        protected IUnitOfWork unitOfWork = null;

        public BaseManager(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("UnitOfWork cannot be null.");
            }
            this.unitOfWork = unitOfWork;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.unitOfWork != null)
                {
                    this.unitOfWork.Dispose();
                    this.unitOfWork = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
