using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Core;
using KeyHub.Core.Mail;
using KeyHub.Model;
using KeyHub.Web.ViewModels.Mail;
using KeyHub.Web.ViewModels.Transaction;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly IMailService mailService;
        public TransactionController(IDataContextFactory dataContextFactory, IMailService mailService)
            : base(dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
            this.mailService = mailService;
        }

        /// <summary>
        /// Get list of transactions
        /// </summary>
        /// <returns>Transaction index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions orderby x.CreatedDateTime select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License));

                var viewModel = new TransactionIndexViewModel(transactionQuery.OrderByDescending(x => x.CreatedDateTime).ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Details of a single transaction
        /// </summary>
        /// <param name="key">Id of the transaction to show</param>
        /// <returns>Transaction details view</returns>
        public ActionResult Details(string key)
        {
            var decryptedKey = Common.Utils.SafeConvert.ToGuid(key.DecryptUrl());

            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions where x.TransactionId == decryptedKey select x)
                    .Include(x => x.IgnoredItems)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .Include(x => x.TransactionItems.Select(s => s.License.Domains))
                    .Include(x => x.TransactionItems.Select(s => s.License.LicenseCustomerApps));

                if (transactionQuery.FirstOrDefault() == null)
                    throw new EntityNotFoundException("Transaction could not be resolved!"); 

                var viewModel = new TransactionDetailsViewModel(transactionQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Partial details view of a single transaction
        /// </summary>
        /// <param name="key">Id of the transaction to show</param>
        /// <returns>Transaction details partial view</returns>
        public ActionResult DetailsPartial(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions where x.TransactionId == key select x)
                    .Include(x => x.IgnoredItems)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .Include(x => x.TransactionItems.Select(s => s.License.Domains));

                var viewModel = new TransactionDetailsViewModel(transactionQuery.FirstOrDefault());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single Transaction
        /// </summary>
        /// <returns>Create transaction view</returns>
        public ActionResult Create()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var basket = BasketWrapper.CreateNewByIdentity(dataContextFactory);
                
                var skuQuery = from x in context.SKUs orderby x.SkuCode select x;

                var viewModel = new TransactionCreateViewModel(basket.Transaction, skuQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created transaction and transactionitems into db and redirect to checkout
        /// </summary>
        /// <param name="viewModel">Created TransactionCreateViewModel</param>
        /// <returns>Redirectaction to checkout if successfull</returns>
        [HttpPost]
        public ActionResult Create(TransactionCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BasketWrapper basket = BasketWrapper.CreateNewByIdentity(dataContextFactory);

                    viewModel.ToEntity(basket.Transaction);
                    
                    basket.AddItems(viewModel.GetSelectedSkuGuids());

                    basket.Transaction.PurchaserEmail = "n/a";
                    basket.Transaction.PurchaserName = "n/a";

                    basket.ExecuteStep(BasketSteps.Create);

                    return RedirectToAction("Checkout", new { key = basket.Transaction.TransactionId.ToString().EncryptUrl() });
                }
                else
                {
                    return View(viewModel);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Claim an existing Transaction into licenses
        /// </summary>
        /// <param name="key">Encrypted key of the transaction to claim</param>
        /// <returns>Claim transaction view</returns>
        [AllowAnonymous]
        public ActionResult ClaimLicenses(string key)
        {
            var transactionId = Common.Utils.SafeConvert.ToGuid(key.DecryptUrl());

            using (var context = dataContextFactory.CreateByTransaction(transactionId))
            {
                var transaction = (from x in context.Transactions where x.TransactionId == transactionId select x)
                    .Include(x => x.IgnoredItems)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .FirstOrDefault();

                if (transaction == null)
                    throw new EntityNotFoundException("Transaction could not be resolved!");

                if (transaction.Status == TransactionStatus.Complete)
                    throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                var viewModel = new TransactionDetailsViewModel(transaction);

                return View(viewModel);
            }

        }

        /// <summary>
        /// Process claimed transaction and proceed to checkout
        /// </summary>
        /// <param name="viewModel">TransactionDetailsViewModel of transaction to claim</param>
        /// <returns>Redirect to checkout</returns>
        [HttpPost]
        public ActionResult ClaimLicenses(TransactionDetailsViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BasketWrapper basket = BasketWrapper.GetByTransactionId(dataContextFactory, viewModel.Transaction.TransactionId);

                    if (basket.Transaction == null)
                        throw new EntityNotFoundException("Transaction could not be resolved!");

                    if (basket.Transaction.Status == TransactionStatus.Complete)
                        throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                    viewModel.ToEntity(basket.Transaction);

                    return RedirectToAction("Checkout", new { key = basket.Transaction.TransactionId.ToString().EncryptUrl() });
                }
                else
                {
                    return View(viewModel);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Create a single Transaction checkout
        /// </summary>
        /// <param name="key">Encrypted key of the transaction to claim</param>
        /// <returns>Transaction checkout view</returns>
        public ActionResult Checkout(string key)
        {
            var transactionId = Common.Utils.SafeConvert.ToGuid(key.DecryptUrl());

            using (var context = dataContextFactory.CreateByUser())
            {
                var basket = BasketWrapper.GetByTransactionId(dataContextFactory, transactionId);

                if (basket.Transaction == null)
                    throw new EntityNotFoundException("Transaction SKUs are not accessible to current user!");

                if (basket.Transaction.Status == TransactionStatus.Complete)
                    throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                var owningCustomerQuery = (from x in context.Customers orderby x.Name select x);
                var purchasingCustomerQuery = (from x in context.Customers orderby x.Name select x);
                var countryQuery = (from x in context.Countries orderby x.CountryName select x);

                var viewModel = new TransactionCheckoutViewModel(basket.Transaction,
                    owningCustomerQuery.ToList(), purchasingCustomerQuery.ToList(), countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save transaction checkout into db and redirect to ...
        /// </summary>
        /// <param name="viewModel">Created TransactionCheckoutViewModel</param>
        /// <returns>Redirect to purchase if successfull</returns>
        [HttpPost]
        public ActionResult Checkout(TransactionCheckoutViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    BasketWrapper basket = BasketWrapper.GetByTransactionId(dataContextFactory, viewModel.Transaction.TransactionId);

                    if (basket.Transaction == null)
                        throw new EntityNotFoundException("Transaction SKUs are not accessible to current user!");

                    if (basket.Transaction.Status == TransactionStatus.Complete)
                        throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                    viewModel.ToEntity(basket.Transaction);

                    if (viewModel.ExistingPurchasingCustomer)
                        basket.PurchasingCustomer = (from x in context.Customers where x.ObjectId == viewModel.PurchasingCustomerId select x).FirstOrDefault();
                    else
                        basket.PurchasingCustomer = viewModel.NewPurchasingCustomer.ToEntity(null);

                    if (viewModel.OwningCustomerIsPurchasingCustomerId)
                    {
                        basket.OwningCustomer = basket.PurchasingCustomer;
                    }
                    else
                    {
                        if (viewModel.ExistingOwningCustomer)
                            basket.OwningCustomer = (from x in context.Customers where x.ObjectId == viewModel.OwningCustomerId select x).FirstOrDefault();    
                        else
                            basket.OwningCustomer = viewModel.NewOwningCustomer.ToEntity(null);    
                            
                    }

                    basket.ExecuteStep(BasketSteps.Checkout);

                    return RedirectToAction("Complete", new { key = basket.Transaction.TransactionId.ToString().EncryptUrl() });
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        /// <summary>
        /// Report transaction complete
        /// </summary>
        /// <param name="key">Encoded key for the transaction</param>
        /// <returns>Transaction complete view</returns>
        /// <remarks>
        /// Does not use cookie. Cookie is removed before this step so a new transaction can 
        /// be started once the transaction has become pending.
        /// </remarks>
        public ActionResult Complete(string key)
        {
            var transactionId = Common.Utils.SafeConvert.ToGuid(key.DecryptUrl());

            var basket = BasketWrapper.GetByTransactionId(dataContextFactory, transactionId);

            basket.ExecuteStep(BasketSteps.Complete);

            var viewModel = new TransactionDetailsViewModel(basket.Transaction);

            return View(viewModel);
        }

        /// <summary>
        /// Resend an email to claim transaction 
        /// </summary>
        /// <param name="key">Id of the transaction</param>
        /// <returns>Result of sending</returns>
        public ActionResult Remind(string key)
        {
            var transactionId = Common.Utils.SafeConvert.ToGuid(key.DecryptUrl());

            BasketWrapper basket = BasketWrapper.GetByTransactionId(dataContextFactory, transactionId);

            if (basket.Transaction == null)
                throw new EntityNotFoundException("Transaction could not be resolved!");

            basket.ExecuteStep(BasketSteps.Remind);

            mailService.SendTransactionMail(basket.Transaction.PurchaserName,
                                            basket.Transaction.PurchaserEmail,
                                            basket.Transaction.TransactionId);

            return RedirectToAction("Index");
        }

    }
}
