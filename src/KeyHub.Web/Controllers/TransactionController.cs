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
        /// Create a single Transaction
        /// </summary>
        /// <returns>Create transaction view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext())
            {
                BasketWrapper basket = new BasketWrapper();
                
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
                    BasketWrapper basket = new BasketWrapper();
                    
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
                BasketWrapper basket = new BasketWrapper();

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
        /// <returns>Redirectaction to ... if successfull</returns>
        [HttpPost]
        public ActionResult Checkout(TransactionCheckoutViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext())
                    {
                        BasketWrapper basket = new BasketWrapper();

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
                BasketWrapper basket = new BasketWrapper();
                basket.ExecuteStep(BasketSteps.Complete);

                TransactionViewModel viewModel = new TransactionViewModel(basket.Transaction);

                return View(viewModel);
            }
        }
    }
}
