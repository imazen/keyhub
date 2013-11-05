using System;
using KeyHub.Data;
using KeyHub.Model;
using Moq;

namespace KeyHub.Tests.TestCore
{
    public class FakeDataContextFactory : IDataContextFactory
    {
        public FakeDataContextFactory()
        {
            DataContext = new Mock<IDataContextByTransaction>();
            DataContext.Setup(x => x.Users).Returns(new FakeDbSet<User>());
            DataContext.Setup(x => x.Countries).Returns(new FakeDbSet<Country>());
            DataContext.Setup(x => x.Features).Returns(new FakeDbSet<Feature>());
            DataContext.Setup(x => x.PrivateKeys).Returns(new FakeDbSet<PrivateKey>());
            DataContext.Setup(x => x.SKUs).Returns(new FakeDbSet<SKU>());
            DataContext.Setup(x => x.Customers).Returns(new FakeDbSet<Customer>());
            DataContext.Setup(x => x.Transactions).Returns(new FakeDbSet<Transaction>());
            DataContext.Setup(x => x.TransactionItems).Returns(new FakeDbSet<TransactionItem>());
            DataContext.Setup(x => x.Vendors).Returns(new FakeDbSet<Vendor>());
            DataContext.Setup(x => x.Licenses).Returns(new FakeDbSet<License>());
            DataContext.Setup(x => x.Rights).Returns(new FakeDbSet<Right>());
            DataContext.Setup(x => x.UserVendorRights).Returns(new FakeDbSet<UserVendorRight>());
            DataContext.Setup(x => x.UserCustomerRights).Returns(new FakeDbSet<UserCustomerRight>());
            DataContext.Setup(x => x.UserLicenseRights).Returns(new FakeDbSet<UserLicenseRight>());
            DataContext.Setup(x => x.DomainLicenses).Returns(new FakeDbSet<DomainLicense>());
            DataContext.Setup(x => x.CustomerApps).Returns(new FakeDbSet<CustomerApp>());
            DataContext.Setup(x => x.LicenseCustomerApps).Returns(new FakeDbSet<LicenseCustomerApp>());
            DataContext.Setup(x => x.CustomerAppKeys).Returns(new FakeDbSet<CustomerAppKey>());
        }

        public Mock<IDataContextByTransaction> DataContext;

        public IDataContext Create()
        {
            return DataContext.Object;
        }

        public IDataContextByUser CreateByUser()
        {
            return DataContext.Object;
        }

        public IDataContextByTransaction CreateByTransaction(Guid transactionId)
        {
            return DataContext.Object;
        }

        public void Release(IDataContext dataContext)
        {
        }
    }
}
