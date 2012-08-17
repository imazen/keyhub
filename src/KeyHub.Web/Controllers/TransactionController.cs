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

namespace KeyHub.Web.Controllers
{
    public class TransactionController : Controller
    {
        /// <summary>
        /// Get list of transactions
        /// </summary>
        /// <returns>Transaction index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext())
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License));

                TransactionIndexViewModel viewModel = new TransactionIndexViewModel(transactionQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Details of a single transaction
        /// </summary>
        /// <param name="key">Id of the transaction to show</param>
        /// <returns>Transaction details view</returns>
        public ActionResult Details(int key)
        {
            using (DataContext context = new DataContext())
            {
                //Eager loading Transaction
                var transactionQuery = (from x in context.Transactions where x.TransactionId == key select x)
                    .Include(x => x.TransactionItems.Select(s => s.Sku))
                    .Include(x => x.TransactionItems.Select(s => s.License));

                TransactionDetailsViewModel viewModel = new TransactionDetailsViewModel(transactionQuery.FirstOrDefault());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single Transaction
        /// </summary>
        /// <returns>Create transaction view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext())
            {
                BasketWrapper basket = BasketWrapper.GetByCookie();
                
                var skuQuery = from x in context.SKUs select x;

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
                    BasketWrapper basket = BasketWrapper.GetByCookie();
                    
                    basket.AddSKUs(viewModel.GetSelectedSKUGUIDs());
                    basket.ExecuteStep(BasketSteps.Create);

                    return RedirectToAction("Checkout");
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
        /// <returns>Transaction checkout view</returns>
        public ActionResult Checkout()
        {
            using (DataContext context = new DataContext())
            {
                BasketWrapper basket = BasketWrapper.GetByCookie();

                var owningCustomerQuery = (from x in context.Customers select x);
                var purchasingCustomerQuery = (from x in context.Customers select x);
                var countryQuery = (from x in context.Countries select x);

                TransactionCheckoutViewModel viewModel = new TransactionCheckoutViewModel(basket.Transaction,
                    owningCustomerQuery.ToList(), purchasingCustomerQuery.ToList(), countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save transaction checkout into db and redirect to ...
        /// </summary>
        /// <param name="viewModel">Created TransactionCheckoutViewModel</param>
        /// <returns>Redirectaction to purchase if successfull</returns>
        [HttpPost]
        public ActionResult Checkout(TransactionCheckoutViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        BasketWrapper basket = BasketWrapper.GetByCookie();

                        viewModel.ToEntity(basket.Transaction);

                        if (viewModel.CreatePurchasingCustomer)
                            basket.PurchasingCustomer = viewModel.NewPurchasingCustomer.ToEntity(null);
                        else
                            basket.PurchasingCustomer = (from x in context.Customers where x.ObjectId == viewModel.PurchasingCustomerId select x).FirstOrDefault();

                        if (viewModel.OwingCustomerIsPurchasingCustomerId)
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

                        return RedirectToAction("Purchase");
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
        /// Purchase transaction through e-commerce
        /// </summary>
        /// <returns>Purchase transaction view</returns>
        public ActionResult Purchase()
        {
            using (DataContext context = new DataContext())
            {
                BasketWrapper basket = BasketWrapper.GetByCookie();

                TransactionViewModel viewModel = new TransactionViewModel(basket.Transaction);

                return View(viewModel);
            }
        }

        /// <summary>
        /// Process transaction transaction purchase
        /// </summary>
        /// <param name="viewModel">Created TransactionViewModel</param>
        /// <returns>Redirectaction to complete if successfull</returns>
        [HttpPost]
        public ActionResult Purchase(TransactionViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        BasketWrapper basket = BasketWrapper.GetByCookie();

                        basket.ExecuteStep(BasketSteps.Purchase);

                        return RedirectToAction("Complete");
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
        /// <returns>Transaction complete view</returns>
        public ActionResult Complete()
        {
            using (DataContext context = new DataContext())
            {
                BasketWrapper basket = BasketWrapper.GetByCookie();
                
                basket.ExecuteStep(BasketSteps.Complete);

                TransactionViewModel viewModel = new TransactionViewModel(basket.Transaction);

                return View(viewModel);
            }
        }

    }
}
