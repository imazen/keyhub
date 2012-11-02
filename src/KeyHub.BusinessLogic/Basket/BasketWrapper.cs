﻿using System;
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

        private BasketWrapper(IIdentity userIdentity, int transactionId)
        {
            context = new DataContext(userIdentity, transactionId);
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
        public static BasketWrapper CreateNewByIdentity(IIdentity userIdentity)
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
            BasketWrapper basket = new BasketWrapper(userIdentity, transactionId);
            bool transactionLoaded = basket.LoadTransaction(transactionId);

            if (!transactionLoaded)
            {
                throw new Exception();
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
        /// Purchaser name
        /// </summary>
        public string PurchaserName
        {
            get; 
            set;
        }

        /// <summary>
        /// Purchaser email
        /// </summary>
        public string PurchaserEmail
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
                    if (!Transaction.TransactionItems.Any())
                        throw new InvalidPropertyException("No transaction items set");

                    //Add transaction if none existing
                    if (!(from x in context.Transactions where x.TransactionId == this.Transaction.TransactionId select x).Any())
                        context.Transactions.Add(Transaction);
                    Transaction.Status = TransactionStatus.Create;
                    Transaction.CreatedDateTime = DateTime.Now;
                    context.SaveChanges();

                    break;
                case BasketSteps.Checkout:
                    var currentUser = context.GetUserByIdentity(HttpContext.Current.User.Identity);

                    //Add PurchasingCustomer if none existing
                    if (this.PurchasingCustomer.ObjectId == new Guid())
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
                        if (this.OwningCustomer.ObjectId == new Guid())
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
                    foreach (var item in Transaction.TransactionItems)
                    {
                        if (item.License == null)
                        {
                            var newLicense = new Model.License()
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
                    context.SaveChanges();

                    var existingCustomerApps = (from x in context.Licenses
                                join y in context.LicenseCustomerApps on x.ObjectId equals y.LicenseId
                                where x.OwningCustomerId == this.OwningCustomer.ObjectId
                                select y.CustomerApp).ToList();

                    //Add to any existing apps
                    if (existingCustomerApps.Any())
                    {
                        foreach (var customerApp in existingCustomerApps)
                        {
                            customerApp.AddLicenses((from x in Transaction.TransactionItems select x.License.ObjectId));
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        //Create default application containing all licenses
                        var newCustomerApp = new Model.CustomerApp()
                                                 {
                                                     ApplicationName = this.OwningCustomer.Name + "_Application"
                                                 };
                        context.CustomerApps.Add(newCustomerApp);
                        newCustomerApp.AddLicenses((from x in Transaction.TransactionItems select x.License.ObjectId));
                        context.SaveChanges();

                        //Create customer application key
                        var customerAppKey = new Model.CustomerAppKey()
                                                 {
                                                     CustomerAppId = newCustomerApp.CustomerAppId
                                                 };
                        context.CustomerAppKeys.Add(customerAppKey);
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
                    .Include(x => x.TransactionItems.Select(s => s.License.Domains))
                    .Include(x => x.TransactionItems.Select(s => s.License.LicenseCustomerApps))
                    .FirstOrDefault();

            return (Transaction != null);
        }
    }
}
