using System;
using System.Security.Principal;

namespace KeyHub.Data
{
    public interface IDataContextFactory
    {
        IDataContext Create();
        IDataContextByUser CreateByUser();
        IDataContextByTransaction CreateByTransaction(Guid transactionId);
        void Release(IDataContext dataContext);
    }
}
