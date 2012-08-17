using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KeyHub.Common;
using KeyHub.Common.Utils;
using KeyHub.Model;
using KeyHub.Runtime;
using KeyHub.Data;

namespace KeyHub.BusinessLogic.Basket
{
    /// <summary>
    /// Wrapper around basket.
    /// Takes care of the order and actual steps a basket goes through during purchase.
    /// Every step ends with a commit into the DB.
    /// </summary>
    public class BasketWrapper
    {
        private BasketWrapper()
        {
            context = new DataContext();
        }

        /// <summary>
        /// DataContext the basket is working with
        /// </summary>
        private DataContext context;

        /// <summary>
        /// Get a basketwrapper based the cookievalue
        /// </summary>
        /// <returns>An instance of a basketwrapper serving the transaction from the cookie</returns>
        public static BasketWrapper GetByCookie()
        {
            int transactionId = HasBasketCookie() ? GetBasketId() : 0;

            BasketWrapper basket = new BasketWrapper();
            basket.LoadTransaction(transactionId);

            return basket;
        }

        /// <summary>
        /// Get a basketwrapper based on a provided transactionId
        /// </summary>
        /// <param name="transactionId">Id of the transaction to load in the basket</param>
        /// <returns>An instance of a basketwrapper serving the transaction</returns>
        public static BasketWrapper GetByTransactionId(int transactionId)
        {
            BasketWrapper basket = new BasketWrapper();
            basket.LoadTransaction(transactionId);

            return basket;
        }

        /// <summary>
        /// Get transaction
        /// </summary>
        public Transaction Transaction
        {
            get;
            private set;
        }

        /// <summary>
        /// Get purchasing customer
        /// </summary>
        public Customer PurchasingCustomer
        {
            get;
            set;
        }

        /// <summary>
        /// Get owning customer
        /// </summary>
        public Customer OwningCustomer
        {
            get;
            set;
        }

        /// <summary>
        /// Execute actions based on current step
        /// </summary>
        /// <param name="step">Step to execute</param>
        public void ExecuteStep(BasketSteps step)
        {
            switch (step)
            {
                case BasketSteps.Create:
                    //Add transaction if none existing
                    if (context.Entry(Transaction).State == System.Data.EntityState.Detached)
                        context.Transactions.Add(Transaction);
                    Transaction.Status = TransactionStatus.Create;
                    Transaction.CreatedDateTime = DateTime.Now;
                    break;
                case BasketSteps.Checkout:
                    //Add PurchasingCustomer if none existing
                    if (context.Entry(PurchasingCustomer).State == System.Data.EntityState.Detached)
                        context.Customers.Add(PurchasingCustomer);

                    //Add OwningCustomer if none existing
                    if (context.Entry(OwningCustomer).State == System.Data.EntityState.Detached)
                        context.Customers.Add(OwningCustomer);

                    //Create licenses for every transactionitem
                    foreach (TransactionItem item in Transaction.TransactionItems)
                    {
                        if (item.License == null)
                        {
                            License newLicense = new Model.License()
                            {
                                SkuId = item.SkuId,
                                PurchasingCustomer = PurchasingCustomer,
                                OwningCustomer = OwningCustomer,
                                OwnerName = OwningCustomer.Name,
                                LicenseIssued = DateTime.Now,
                                LicenseExpires = DateTime.Now.AddYears(1)
                            };
                            context.Licenses.Add(newLicense);

                            item.License = newLicense;
                        }
                    }
                    Transaction.Status = TransactionStatus.CheckoutComplete;
                    break;
                case BasketSteps.PurchaseStart:
                    Transaction.Status = TransactionStatus.PurchaseStart;
                    break;
                case BasketSteps.PurchasePending:
                    Transaction.Status = TransactionStatus.PurchasePending;
                    break;
                case BasketSteps.Complete:
                    Transaction.Status = TransactionStatus.Complete;
                    break;
            }
            
            context.SaveChanges();

            //Save cookie while purchase is being created, if pending or higher remove cookie
            //After status pending is reached, a new transaction can be started by the user
            if (step < BasketSteps.PurchasePending)
                SaveBasketId(Transaction.TransactionId);
            else
                RemoveBasketCookie();
        }

        /// <summary>
        /// Add a list of SKUs to the current transaction
        /// </summary>
        /// <param name="SKUs">SKUs to add</param>
        public void AddSKUs(ICollection<Guid> SKUs)
        {
            //Offload adding TransactionItems to Dynamic Transaction Model
            Transaction.AddTransactionItems(SKUs);
        }

        /// <summary>
        /// Load a certain transaction into the BasketWrapper
        /// </summary>
        private void LoadTransaction(int transactionId)
        {
            //Select transaction
            if (transactionId > 0)
                Transaction = (from x in context.Transactions where x.TransactionId == transactionId select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .FirstOrDefault();

            //No transaction in cookie or transaction was not found
            if (Transaction == null)
                Transaction = new Transaction() { CreatedDateTime = DateTime.Now };
        }

        #region "BasketId from Cookie"
        private void SaveBasketId(int transactionId)
        {
            CookieUtil.CreateCookie(HttpContext.Current,
                                    Constants.BasketCookieName,
                                    transactionId.ToString(),
                                    Constants.BasketCookieExpirationDays,
                                    true);
        }

        private void RemoveBasketCookie()
        {
            CookieUtil.DeleteCookie(HttpContext.Current,
                                    Constants.BasketCookieName);
        }

        private static int GetBasketId()
        {
            string basketCookie = CookieUtil.GetCookieValue(HttpContext.Current,
                                                              Constants.BasketCookieName,
                                                              true);

            int basketId = 0;
            return int.TryParse(basketCookie, out basketId) ? basketId : 0;
        }

        private static bool HasBasketCookie()
        {
            return CookieUtil.CookieExists(HttpContext.Current, Constants.BasketCookieName);
        }
        #endregion
    }
}
