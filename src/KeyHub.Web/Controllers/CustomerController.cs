using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.Customer;
using KeyHub.Data;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the Customer entity
    /// </summary>
    public class CustomerController : ControllerBase
    {
        /// <summary>
        /// Get list of Customers
        /// </summary>
        /// <returns>Customer index list view</returns>
        public ActionResult Index()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                //Eager loading Customer
                var customerQuery = (from x in context.Customers select x).Include(x => x.Country);//.FilterByUser(UserEntity);

                CustomerIndexViewModel viewModel = new CustomerIndexViewModel(customerQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single Customer
        /// </summary>
        /// <returns>Create Customer view</returns>
        public ActionResult Create()
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                var countryQuery = from x in context.Countries select x;

                CustomerCreateViewModel viewModel = new CustomerCreateViewModel(countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created Customer into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created CustomerCreateViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Create(CustomerCreateViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext(User.Identity))
                    {
                        Model.Customer customer = viewModel.ToEntity(null);
                        context.Customers.Add(customer);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Index");
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
        /// Edit a single Customer
        /// </summary>
        /// <param name="key">Key of the customer to edit</param>
        /// <returns>Edit Customer view</returns>
        public ActionResult Edit(Guid key)
        {
            using (DataContext context = new DataContext(User.Identity))
            {
                var customerQuery = from x in context.Customers where x.ObjectId == key select x;
                var countryQuery = from x in context.Countries select x;

                CustomerEditViewModel viewModel = new CustomerEditViewModel(customerQuery.FirstOrDefault(),
                    countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited Customer into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited CustomerEditViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost]
        public ActionResult Edit(CustomerEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DataContext context = new DataContext(User.Identity))
                    {
                        Model.Customer customer = (from x in context.Customers where x.ObjectId == viewModel.Customer.ObjectId select x).FirstOrDefault();
                        viewModel.ToEntity(customer);

                        context.SaveChanges();
                    }
                    return RedirectToAction("Index");
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
    }
}
