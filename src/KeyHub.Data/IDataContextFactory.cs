using System.Security.Principal;

namespace KeyHub.Data
{
    public interface IDataContextFactory
    {
        IDataContext Create();
        IDataContextByUser CreateByUser();
        IDataContextByTransaction CreateByTransaction(int transactionId);
        void Release(IDataContext dataContext);
    }
}
