using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Model;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.Transaction;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class TransactionController : Controller
    {
        /// <summary>
        /// Get list of transactions
        /// </summary>
        /// <returns>Transaction index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions orderby x.CreatedDateTime select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .OrderByDescending(x => x.CreatedDateTime);

                TransactionIndexViewModel viewModel = new TransactionIndexViewModel(transactionQuery.ToList());

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
            int decryptedKey = Common.Utils.SafeConvert.ToInt(key.DecryptUrl(), -1);

            using (DataContext context = new DataContext(User.Identity))
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions where x.TransactionId == decryptedKey select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .Include(x => x.TransactionItems.Select(s => s.License.Domains))
                    .Include(x => x.TransactionItems.Select(s => s.License.LicenseCustomerApps));

                if (transactionQuery.FirstOrDefault() == null)
                    throw new EntityNotFoundException("Transaction could not be resolved!"); 

                TransactionDetailsViewModel viewModel = new TransactionDetailsViewModel(transactionQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Partial details view of a single transaction
        /// </summary>
        /// <param name="key">Id of the transaction to show</param>
        /// <returns>Transaction details partial view</returns>
        public ActionResult DetailsPartial(int key)
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions where x.TransactionId == key select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .Include(x => x.TransactionItems.Select(s => s.License.Domains));

                TransactionDetailsViewModel viewModel = new TransactionDetailsViewModel(transactionQuery.FirstOrDefault());

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Create a single Transaction
        /// </summary>
        /// <returns>Create transaction view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                BasketWrapper basket = BasketWrapper.CreateNewByIdentity(User.Identity);
                
                var skuQuery = from x in context.SKUs orderby x.SkuCode select x;

                TransactionCreateViewModel viewModel = new TransactionCreateViewModel(basket.Transaction, skuQuery.ToList());

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
                    BasketWrapper basket = BasketWrapper.CreateNewByIdentity(User.Identity);

                    viewModel.ToEntity(basket.Transaction);
                    
                    basket.AddSKUs(viewModel.GetSelectedSKUGUIDs());
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
            int transactionID = Common.Utils.SafeConvert.ToInt(key.DecryptUrl(), -1);

            using (DataContext context = new DataContext(transactionID))
            {
                var transaction = (from x in context.Transactions where x.TransactionId == transactionID select x)
                                  .Include(x => x.TransactionItems.Select(s => s.Sku))
                                  .Include(x => x.TransactionItems.Select(s => s.License))
                                  .FirstOrDefault();
                    
                if (transaction == null)
                    throw new EntityNotFoundException("Transaction could not be resolved!"); 

                if (transaction.Status != TransactionStatus.Create)
                    throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                TransactionDetailsViewModel viewModel = new TransactionDetailsViewModel(transaction);

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
                    BasketWrapper basket = BasketWrapper.GetByTransactionId(User.Identity, viewModel.Transaction.TransactionId);

                    if (basket.Transaction == null)
                        throw new EntityNotFoundException("Transaction could not be resolved!");

                    if (basket.Transaction.Status != TransactionStatus.Create)
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
            int transactionID = Common.Utils.SafeConvert.ToInt(key.DecryptUrl(), -1);

            using (DataContext context = new DataContext(User.Identity))
            {
                BasketWrapper basket = BasketWrapper.GetByTransactionId(User.Identity, transactionID);

                if (basket.Transaction == null)
                    throw new EntityNotFoundException("Transaction SKUs are not accessible to current user!");

                if (basket.Transaction.Status != TransactionStatus.Create)
                    throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                var owningCustomerQuery = (from x in context.Customers orderby x.Name select x);
                var purchasingCustomerQuery = (from x in context.Customers orderby x.Name select x);
                var countryQuery = (from x in context.Countries orderby x.CountryName select x);

                TransactionCheckoutViewModel viewModel = new TransactionCheckoutViewModel(basket.Transaction,
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
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext(User.Identity))
                    {
                        BasketWrapper basket = BasketWrapper.GetByTransactionId(User.Identity, viewModel.Transaction.TransactionId);

                        if (basket.Transaction == null)
                            throw new EntityNotFoundException("Transaction SKUs are not accessible to current user!");

                        if (basket.Transaction.Status != TransactionStatus.Create)
                            throw new EntityOperationNotSupportedException("Transaction is already claimed!");

                        viewModel.ToEntity(basket.Transaction);

                        if (viewModel.CreatePurchasingCustomer)
                            basket.PurchasingCustomer = viewModel.NewPurchasingCustomer.ToEntity(null);
                        else
                            basket.PurchasingCustomer = (from x in context.Customers where x.ObjectId == viewModel.PurchasingCustomerId select x).FirstOrDefault();

                        if (viewModel.OwningCustomerIsPurchasingCustomerId)
                        {
                            basket.OwningCustomer = basket.PurchasingCustomer;
                        }
                        else
                        {
                            if (viewModel.CreateOwningCustomer)
                                basket.OwningCustomer = viewModel.NewOwningCustomer.ToEntity(null);
                            else
                                basket.OwningCustomer = (from x in context.Customers where x.ObjectId == viewModel.OwningCustomerId select x).FirstOrDefault();
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
            catch
            {
                throw;
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
            int transactionID = Common.Utils.SafeConvert.ToInt(key.DecryptUrl(), -1);
            using (DataContext context = new DataContext(User.Identity))
            {
                BasketWrapper basket = BasketWrapper.GetByTransactionId(User.Identity, transactionID);
                
                basket.ExecuteStep(BasketSteps.Complete);

                TransactionDetailsViewModel viewModel = new TransactionDetailsViewModel(basket.Transaction);

                return View(viewModel);
            }
        }

    }
}
