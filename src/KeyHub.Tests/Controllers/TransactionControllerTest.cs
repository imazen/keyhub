using ActionMailer.Net.Mvc;
using KeyHub.Core.Mail;
using KeyHub.Data;
using KeyHub.Model;
using KeyHub.Tests.TestCore;
using KeyHub.Tests.TestData;
using KeyHub.Web.Api.Controllers;
using KeyHub.Web.Api.Controllers.Transaction;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Xml.Serialization;
using TransactionController = KeyHub.Web.Api.Controllers.TransactionController;

namespace KeyHub.Tests.Controllers
{
    [TestClass]
    public class TransactionControllerTest
    {
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

        private TransactionController controller;
        private FakeDataContextFactory dataContextFactory;
        private Mock<FakeMailService> mailService;

        private const string purchaserName = "Test Test";
        private const string purchaserEmail = "test@test.test";
        private Guid purchasedSkuId;
        private TransactionRequest transactionRequest;
        private Feature feature1;
        private Feature feature2;
        private PrivateKey privateKey1;
        private SKU sku1;
        private Transaction transaction;


        // ReSharper restore InconsistentNaming

        // ReSharper disable ImplicitlyCapturedClosure

        [TestInitialize]
        public void Initialize()
        {
            purchasedSkuId = Guid.NewGuid();

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

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ToXmlString(transactionRequest)),
                new HttpResponse(new StringWriter())
            );

            mailService = new Mock<FakeMailService>();

            dataContextFactory = new FakeDataContextFactory();

            dataContextFactory.DataContext
                .Setup(x => x.GetUser(It.IsAny<IIdentity>()))
                .Returns(UserTestData.CreateAnonymous());

            dataContextFactory.DataContext
                  .Setup(x => x.SKUs)
                  .Returns(new FakeDbSet<SKU>
                                  {
                                      sku1
                                  });

            controller = new TransactionController(dataContextFactory, mailService.Object);
        }

        [TestMethod]
        public void ShouldCreateTransactionWithTransactionItemsAndSendEmail()
        {
            var transactionResult = controller.Post(transactionRequest);

            Assert.IsNotNull(transactionResult);
            Assert.IsTrue(transactionResult.CreatedSuccessfull);

            var createdTransaction = dataContextFactory.DataContext.Object.Transactions.FirstOrDefault();
            Assert.IsNotNull(createdTransaction);
            Assert.IsTrue(createdTransaction.PurchaserName == purchaserName);
            Assert.IsTrue(createdTransaction.PurchaserEmail == purchaserEmail);
            Assert.IsTrue(createdTransaction.Status == TransactionStatus.Create);
            Assert.IsTrue(createdTransaction.TransactionItems.Any());

            mailService.Verify(x => x.SendTransactionMail(purchaserName, purchaserEmail, It.IsAny<Guid>()), Times.Once());
        }

        //[TestMethod]
        //public void ShouldNotPassSkuExpirationRule()
        //{
        //    sku1.ExpirationDate = DateTime.Today.AddDays(-1);

        //    Mock.Arrange(() => context.SKUs)
        //       .IgnoreInstance()
        //       .ReturnsCollection(new List<SKU> { sku1 });

        //    Mock.Arrange(() => context.Transactions)
        //        .IgnoreInstance()
        //        .ReturnsCollection(new List<Transaction> { });

        //    Mock.Arrange(() => context.Transactions.Add(Arg.Matches<Transaction>(t => t.PurchaserName == purchaserName && t.PurchaserEmail == purchaserEmail)))
        //        .IgnoreInstance()
        //        .DoInstead<Transaction>(t => transaction = t);

        //    Mock.Arrange(() => context.SaveChanges())
        //        .IgnoreInstance()
        //        .DoInstead(() => context.ValidateModelItem(transaction.TransactionItems.First()));

        //    Mock.Initialize<MailController>();
        //    var mailController = new MailController();
        //    var emailResult = Mock.Create<EmailResult>(Constructor.Mocked);

        //    Mock.Arrange(() => mailController.TransactionEmail(Arg.Matches<TransactionMailViewModel>(t => t.PurchaserName == purchaserName && t.PurchaserEmail == purchaserEmail)))
        //        .IgnoreInstance()
        //        .Returns(emailResult)
        //        .OccursNever();

        //    Mock.Arrange(() => emailResult.Deliver())
        //        .DoNothing()
        //        .OccursNever();

        //    IDataContextFactory factory = null;//Mock factory
        //    IMailService mailService = null;
        //    TransactionResult transactionResult = new TransactionController(factory, mailService).Post(transactionRequest);

        //    Mock.AssertAll(context);
        //    Mock.AssertAll(mailController);
        //    Mock.AssertAll(emailResult);

        //    Assert.IsNotNull(transactionResult);
        //    Assert.IsFalse(transactionResult.CreatedSuccessfull);
        //}

        private static string ToXmlString<T>(T obj)
        {
            var stringWriter = new StringWriter();
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }
    }
}
