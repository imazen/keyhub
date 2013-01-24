using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Xml.Serialization;
using ActionMailer.Net.Mvc;
using KeyHub.Data;
using KeyHub.Model;
using KeyHub.Tests.TestData;
using KeyHub.Web.Api.Controllers.Transaction;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using TransactionController = KeyHub.Web.Api.Controllers.TransactionController;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class TransactionControllerTest
    {
        // ReSharper disable InconsistentNaming

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private string purchaserName;
        private string purchaserEmail;
        private Guid purchasedSkuId;

        private TransactionRequest transactionRequest;

        private Feature feature1;
        private Feature feature2;
        private PrivateKey privateKey1;
        private SKU sku1;
        private Transaction transaction;

        private DataContext context;

        // ReSharper restore InconsistentNaming

        // ReSharper disable ImplicitlyCapturedClosure

        [TestInitialize]
        public void Initialize()
        {
            purchaserName = "Test Test";
            purchaserEmail = "test@test.test";
            purchasedSkuId = GuidTestData.Create(10);

            transactionRequest = new TransactionRequest
            {
                PurchaserName = purchaserName,
                PurchaserEmail = purchaserEmail,
                PurchasedSkus = new[] { purchasedSkuId }
            };

            feature1 = FeatureTestData.Create(GuidTestData.Create(1));
            feature2 = FeatureTestData.Create(GuidTestData.Create(2));
            privateKey1 = PrivateKeyTestData.Create();
            sku1 = SkuTestData.Create(privateKey1, feature1, feature2);
            sku1.SkuId = purchasedSkuId;

            //Prepare HttpContext and Principal
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ToXmlString(transactionRequest)),
                new HttpResponse(new StringWriter())
            );
            HttpContext.Current.User = new GenericPrincipal(
                new GenericIdentity(""),
                new string[0]
                );

            //Mock datacontext
            Mock.Initialize<DataContext>();
            context = new DataContext();

            // Mock user instance to divert role manager
            var userMock = Mock.Create<User>();
            Mock.Arrange(() => userMock.IsVendorAdmin).IgnoreInstance().Returns(false);
            Mock.Arrange(() => userMock.IsSystemAdmin).IgnoreInstance().Returns(false);
        }

        [TestMethod]
        public void ShouldCreateDbStuffAndSendEmail()
        {
            Mock.Arrange(() => context.SKUs)
               .IgnoreInstance()
               .ReturnsCollection(new List<SKU> { sku1 });

            Mock.Arrange(() => context.Transactions)
                .IgnoreInstance()
                .ReturnsCollection(new List<Transaction> {});

            Mock.Arrange(() => context.Transactions.Add(Arg.Matches<Transaction>(t => t.PurchaserName == purchaserName && t.PurchaserEmail == purchaserEmail)))
                .IgnoreInstance()
                .DoInstead<Transaction>(t => transaction = t);

            Mock.Arrange(() => context.SaveChanges())
                .IgnoreInstance()
                .DoInstead(() => context.ValidateModelItem(transaction.TransactionItems.First()));

            Mock.Initialize<MailController>();
            var mailController = new MailController();
            var emailResult = Mock.Create<EmailResult>(Constructor.Mocked);

            Mock.Arrange(() => mailController.TransactionEmail(Arg.Matches<TransactionMailViewModel>(t => t.PurchaserName == purchaserName && t.PurchaserEmail == purchaserEmail)))
                .IgnoreInstance()
                .Returns(emailResult)
                .OccursOnce();

            Mock.Arrange(() => emailResult.Deliver())
                .DoNothing()
                .OccursOnce();

            TransactionResult transactionResult = new TransactionController().Post(transactionRequest);

            Mock.AssertAll(context);
            Mock.AssertAll(mailController);
            Mock.AssertAll(emailResult);

            Assert.IsNotNull(transactionResult);
            Assert.IsTrue(transactionResult.CreatedSuccessfull);
        }

        [TestMethod]
        public void ShouldNotPassSkuExpirationRule()
        {
            sku1.ExpirationDate = DateTime.Today.AddDays(-1);

            Mock.Arrange(() => context.SKUs)
               .IgnoreInstance()
               .ReturnsCollection(new List<SKU> { sku1 });

            Mock.Arrange(() => context.Transactions)
                .IgnoreInstance()
                .ReturnsCollection(new List<Transaction> { });

            Mock.Arrange(() => context.Transactions.Add(Arg.Matches<Transaction>(t => t.PurchaserName == purchaserName && t.PurchaserEmail == purchaserEmail)))
                .IgnoreInstance()
                .DoInstead<Transaction>(t => transaction = t);

            Mock.Arrange(() => context.SaveChanges())
                .IgnoreInstance()
                .DoInstead(() => context.ValidateModelItem(transaction.TransactionItems.First()));

            Mock.Initialize<MailController>();
            var mailController = new MailController();
            var emailResult = Mock.Create<EmailResult>(Constructor.Mocked);

            Mock.Arrange(() => mailController.TransactionEmail(Arg.Matches<TransactionMailViewModel>(t => t.PurchaserName == purchaserName && t.PurchaserEmail == purchaserEmail)))
                .IgnoreInstance()
                .Returns(emailResult)
                .OccursNever();

            Mock.Arrange(() => emailResult.Deliver())
                .DoNothing()
                .OccursNever();

            TransactionResult transactionResult = new TransactionController().Post(transactionRequest);

            Mock.AssertAll(context);
            Mock.AssertAll(mailController);
            Mock.AssertAll(emailResult);

            Assert.IsNotNull(transactionResult);
            Assert.IsFalse(transactionResult.CreatedSuccessfull);
        }

        // ReSharper enable ImplicitlyCapturedClosure

        private static string ToXmlString<T>(T obj)
        {
            var stringWriter = new StringWriter();
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }
    }
}
