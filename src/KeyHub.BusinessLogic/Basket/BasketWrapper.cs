using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KeyHub.Model;
using KeyHub.Data;
using System.Security.Principal;

namespace KeyHub.BusinessLogic.Basket
{
    public class BasketWrapperBase
    {
        public static BasketWrapperBase CreateNew()
        {
            return new BasketWrapperBase()
            {
                Transaction = new Transaction {CreatedDateTime = DateTime.Now}
            };
        }

        /// <summary>
        /// Get transaction
        /// </summary>
        public Transaction Transaction
        {
            get;
            protected set;
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

        protected void InsertIgnoredItem(string description)
        {
            Transaction.AddIgnoredItem(new TransactionIgnoredItem
            {
                TransactionId = Transaction.TransactionId,
                Description = description
            });
        }

        protected void InsertTransactionItem(Guid skuGuid)
        {
            Transaction.AddTransactionItem(new TransactionItem
            {
                TransactionId = Transaction.TransactionId,
                SkuId = skuGuid
            });
        }

        protected static Guid? ParseGuid(string guid)
        {
            Guid newGuid;

            if (!Guid.TryParse(guid, out newGuid))
                return null;

            return newGuid;
        }

        public void AddItems(IDataContext dataContext, IEnumerable<string> skus, IEnumerable<Guid> authorizedVendorIds)
        {
            foreach (var sku in skus)
            {
                var skuGuid = ParseGuid(sku);

                if (skuGuid.HasValue)
                {
                    if (CheckIfSkuExistsInDatabase(dataContext, skuGuid.Value, authorizedVendorIds))
                        InsertTransactionItem(skuGuid.Value);
                    else
                        InsertIgnoredItem(sku);
                }
                else
                    InsertIgnoredItem(sku);
            }
        }

        protected bool CheckIfSkuExistsInDatabase(IDataContext dataContext, Guid skuGuid, IEnumerable<Guid> authorizedVendorIds)
        {
            //  Available skus will be limited by the dataContext already if its a 
            //  DataContextByUser / DataContextByTransaction
            IEnumerable<SKU> availableSkus = dataContext.SKUs;

            if (authorizedVendorIds != null)
                availableSkus = availableSkus.Where(x => authorizedVendorIds.Contains(x.VendorId));

            return (availableSkus.Any(x => x.SkuId == skuGuid));
        }

        public void ExecuteCreate(IDataContext dataContext)
        {
            if (!Transaction.TransactionItems.Any())
                throw new InvalidPropertyException("No transaction items set");

            //Add transaction if none existing
            if (!(from x in dataContext.Transactions where x.TransactionId == this.Transaction.TransactionId select x).Any())
                dataContext.Transactions.Add(Transaction);
            Transaction.Status = TransactionStatus.Create;
            Transaction.CreatedDateTime = DateTime.Now;
            dataContext.SaveChanges();
        }
    }

    /// <summary>
    /// Wrapper around basket.
    /// Takes care of the order and actual steps a basket goes through during purchase.
    /// Every step ends with a commit into the DB.
    /// </summary>
    public class BasketWrapper : BasketWrapperBase
    {
        private readonly IDataContextFactory dataContextFactory;

        private BasketWrapper(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
            context = dataContextFactory.CreateByUser();
        }

        private BasketWrapper(IDataContextFactory dataContextFactory, Guid transactionId)
        {
            this.dataContextFactory = dataContextFactory;
            context =  dataContextFactory.CreateByTransaction(transactionId);
        }

        /// <summary>
        /// DataContext the basket is working with
        /// </summary>
        private readonly IDataContext context;

        /// <summary>
        /// Start a new basket
        /// </summary>
        /// <param name="dataContextFactory">Data context factory</param>
        /// <returns>An instance of a basketwrapper serving a new transaction</returns>
        public static BasketWrapper CreateNewByIdentity(IDataContextFactory dataContextFactory)
        {
            var basket = new BasketWrapper(dataContextFactory)
                             {Transaction = new Transaction {CreatedDateTime = DateTime.Now}};

            return basket;
        }

        /// <summary>
        /// Get a basketwrapper based on a provided transactionId
        /// </summary>
        /// <param name="dataContextFactory">Data context factory</param>
        /// <param name="transactionId">Id of the transaction to load in the basket</param>
        /// <returns>An instance of a basketwrapper serving the transaction</returns>
        /// <remarks>
        /// At first attempt the a datacontext based on userIdentity is applied.
        /// If userIdentity datacontext does not contain requested transaction, user does not yet
        /// have access to transaction (new user or new vendor). Use datacontext based on 
        /// transactionId instead.
        /// </remarks>
        public static BasketWrapper GetByTransactionId(IDataContextFactory dataContextFactory, Guid transactionId)
        {
            var basket = new BasketWrapper(dataContextFactory, transactionId);
            var transactionLoaded = basket.LoadTransaction(transactionId);

            if (!transactionLoaded)
            {
                throw new Exception();
            }

            return basket;
        }

        public void ExecuteComplete()
        {
            Transaction.Status = TransactionStatus.Complete;
            context.SaveChanges();
        }

        public void ExecuteRemind()
        {
            Transaction.Status = TransactionStatus.Remind;
            context.SaveChanges();
        }

        public void ExecuteCheckout()
        {
            var currentUser = context.GetUser(HttpContext.Current.User.Identity);

            //Add PurchasingCustomer if none existing
            if (this.PurchasingCustomer.ObjectId == new Guid())
            {
                context.Customers.Add(PurchasingCustomer);
                context.SaveChanges();
                if (!currentUser.IsVendorAdmin)
                {
                    context.UserCustomerRights.Add(new UserCustomerRight
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
                        context.UserCustomerRights.Add(new UserCustomerRight
                        {
                            RightObject = OwningCustomer,
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
                    var newLicense = new Model.License(item.Sku, this.OwningCustomer, this.PurchasingCustomer);
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
                var newCustomerApp = new CustomerApp()
                {
                    ApplicationName = this.OwningCustomer.Name + "_Application"
                };
                context.CustomerApps.Add(newCustomerApp);
                newCustomerApp.AddLicenses((from x in Transaction.TransactionItems select x.License.ObjectId));
                context.SaveChanges();

                //Create customer application key
                var customerAppKey = new CustomerAppKey()
                {
                    CustomerAppId = newCustomerApp.CustomerAppId
                };
                context.CustomerAppKeys.Add(customerAppKey);
            }
            Transaction.Status = TransactionStatus.Complete;
        }

        public void ExecuteCreate()
        {
            ExecuteCreate(context);
        }

        /// <summary>
        /// Add SKUs to Transaction based on selected SKU strings.
        /// If the value could not be parsed the string will be added the the ignored items collection
        /// </summary>
        public void AddItems(IEnumerable<string> skus)
        {
            AddItems(context, skus, null);
        }

        /// <summary>
        /// Load a certain transaction into the BasketWrapper
        /// </summary>
        /// <returns>True if transaction could be loaded</returns>
        private bool LoadTransaction(Guid transactionId)
        {
            //Select transaction
            Transaction = (from x in context.Transactions where x.TransactionId == transactionId select x)
                            .Include(x => x.IgnoredItems)
                            .Include(x => x.TransactionItems.Select(s => s.Sku))
                            .Include(x => x.TransactionItems.Select(s => s.License))
                            .Include(x => x.TransactionItems.Select(s => s.License.Domains))
                            .Include(x => x.TransactionItems.Select(s => s.License.LicenseCustomerApps))
                            .FirstOrDefault();

            return (Transaction != null);
        }
    }
}
