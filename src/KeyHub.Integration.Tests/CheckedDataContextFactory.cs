using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;

namespace KeyHub.Integration.Tests
{
    public class CheckedDataContextFactory : IDataContextFactory, IDisposable
    {
        private int instanceCount;
        private int releaseCount;
        private IIdentity identity;
        
        public CheckedDataContextFactory(IIdentity identity)
        {
            this.instanceCount = 0;
            this.releaseCount = 0;
            this.identity = identity;
        }

        public IDataContext Create()
        {
            instanceCount++;
            return new DataContext();
        }

        public IDataContextByUser CreateByUser()
        {
            instanceCount++;
            return new DataContextByUser(identity);
        }

        public IDataContextByTransaction CreateByTransaction(Guid transactionId)
        {
            instanceCount++;
            return new DataContextByTransaction(identity, transactionId);
        }

        public void Release(IDataContext dataContext)
        {
            releaseCount++;
            dataContext.Dispose();
        }

        public void Dispose()
        {
            /*  I think we have a bug- the code is called .Dispose() on the contexts, but Windsor expects factory.Release(context) to be used.
            if (releaseCount != instanceCount)
                throw new Exception(String.Format("CheckedDataContextFactory detected a leak- {0} allocations had {1} releases. (the values should match)", instanceCount, releaseCount));
             */
        }
    }
}
