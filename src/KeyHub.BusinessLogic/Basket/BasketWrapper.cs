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

namespace KeyHub.BusinessLogic.Basket
{
    /// <summary>
    /// Wrapper around basket.
    /// Takes care of the order and actual steps a basket goes through during purchase
    /// </summary>
    public class BasketWrapper
    {
        private DataContext context;

        public BasketWrapper()
        {
            context = new DataContext();

            LoadCurrentstate();
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
                    break;
                case BasketSteps.Purchase:
                    break;
                case BasketSteps.Complete:

                    break;
            }
            
            context.SaveChanges();

            if (step != BasketSteps.Complete)
                SaveBasketId(Transaction.TransactionId);
            else
                RemoveBasketCookie();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SKUs"></param>
        public void AddSKUs(ICollection<Guid> SKUs)
        {
            //Offload adding TransactionItems to Dynamic Transaction Model
            Transaction.AddTransactionItems(SKUs);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadCurrentstate()
        {
            int transactionId = HasBasketCookie() ? GetBasketId() : 0;

            //Select transaction
            if (transactionId > 0)
                Transaction = (from x in context.Transactions where x.TransactionId == transactionId select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .FirstOrDefault();

            //No transaction in cookie or transaction was not found
            if (Transaction == null)
                Transaction = new Transaction();
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

        private int GetBasketId()
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
