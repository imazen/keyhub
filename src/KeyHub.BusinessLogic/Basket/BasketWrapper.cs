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
using System.Security.Principal;
using System.Data;

namespace KeyHub.BusinessLogic.Basket
{
    /// <summary>
    /// Wrapper around basket.
    /// Takes care of the order and actual steps a basket goes through during purchase.
    /// Every step ends with a commit into the DB.
    /// </summary>
    public class BasketWrapper
    {
        private BasketWrapper(IIdentity userIdentity)
        {
            context = new DataContext(userIdentity);
        }

        private BasketWrapper(int transactionId)
        {
            context = new DataContext(transactionId);
        }

        /// <summary>
        /// DataContext the basket is working with
        /// </summary>
        private DataContext context;

        /// <summary>
        /// Start a new basket
        /// </summary>
        /// <param name="userIdentity">Identity of currently logged in use</param>
        /// <returns>An instance of a basketwrapper serving a new transaction</returns>
        public static BasketWrapper CreateNew(IIdentity userIdentity)
        {
            BasketWrapper basket = new BasketWrapper(userIdentity);
            basket.Transaction = new Transaction() { CreatedDateTime = DateTime.Now };

            return basket;
        }

        /// <summary>
        /// Get a basketwrapper based on a provided transactionId
        /// </summary>
        /// <param name="userIdentity">Identity of currently logged in user</param>
        /// <param name="transactionId">Id of the transaction to load in the basket</param>
        /// <returns>An instance of a basketwrapper serving the transaction</returns>
        /// <remarks>
        /// At first attempt the a datacontext based on userIdentity is applied.
        /// If userIdentity datacontext does not contain requested transaction, user does not yet
        /// have access to transaction (new user or new vendor). Use datacontext based on 
        /// transactionId instead.
        /// </remarks>
        public static BasketWrapper GetByTransactionId(IIdentity userIdentity, int transactionId)
        {
            BasketWrapper basket = new BasketWrapper(userIdentity);
            bool transactionLoaded = basket.LoadTransaction(transactionId);

            if (!transactionLoaded)
            {
                basket = new BasketWrapper(transactionId);
                basket.LoadTransaction(transactionId);
            }

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
                    if ((from x in context.Transactions where x.TransactionId == this.Transaction.TransactionId select x).Count() == 0)
                        context.Transactions.Add(Transaction);
                    Transaction.Status = TransactionStatus.Create;
                    Transaction.CreatedDateTime = DateTime.Now;
                    break;
                case BasketSteps.Checkout:
                    User currentUser = context.GetUserByIdentity(HttpContext.Current.User.Identity);
                    
                    //How to check for new
                    //Add PurchasingCustomer if none existing
                    if ((from x in context.Customers where x.ObjectId == this.PurchasingCustomer.ObjectId select x).Count() == 0)
                    {
                        context.Customers.Add(PurchasingCustomer);
                        context.SaveChanges();
                        if (!currentUser.IsVendorAdmin)
                        {
                            context.UserCustomerRights.Add(new UserCustomerRight()
                            {
                                RightObject = PurchasingCustomer,
                                RightId = EditEntityMembers.Id,
                                UserId = currentUser.UserId
                            });
                            context.SaveChanges();
                        }
                    }

                    //Add OwningCustomer if none existing
                    if (this.OwningCustomer != this.PurchasingCustomer)
                    {
                        if ((from x in context.Customers where x.ObjectId == this.OwningCustomer.ObjectId select x).Count() == 0)
                        {
                            context.Customers.Add(OwningCustomer);
                            context.SaveChanges();
                            if (!currentUser.IsVendorAdmin)
                            {
                                context.UserCustomerRights.Add(new UserCustomerRight()
                                {
                                    RightObject = PurchasingCustomer,
                                    RightId = EditEntityInfo.Id,
                                    UserId = currentUser.UserId
                                });
                                context.SaveChanges();
                            }
                        }
                    }

                    //Create licenses for every transactionitem
                    foreach (TransactionItem item in Transaction.TransactionItems)
                    {
                        if (item.License == null)
                        {
                            License newLicense = new Model.License()
                            {
                                SkuId = item.SkuId,
                                PurchasingCustomerId = this.PurchasingCustomer.ObjectId,
                                OwningCustomerId = this.OwningCustomer.ObjectId,
                                OwnerName = this.OwningCustomer.Name,
                                LicenseIssued = DateTime.Now,
                                LicenseExpires = DateTime.Now.AddYears(1)
                            };
                            context.Licenses.Add(newLicense);

                            item.License = newLicense;
                        }
                    }
                    Transaction.Status = TransactionStatus.Complete;
                    break;
                case BasketSteps.Complete:
                    Transaction.Status = TransactionStatus.Complete;
                    break;
            }
            
            context.SaveChanges();
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
        /// <returns>True if transaction could be loaded</returns>
        private bool LoadTransaction(int transactionId)
        {
            //Select transaction
            if (transactionId > 0)
                Transaction = (from x in context.Transactions where x.TransactionId == transactionId select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License))
                    .FirstOrDefault();

            return (Transaction != null);
        }
    }
}
