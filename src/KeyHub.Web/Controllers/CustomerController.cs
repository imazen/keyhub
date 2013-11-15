using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KeyHub.Model;
using KeyHub.Web.ViewModels.Customer;
using KeyHub.Data;
using MvcFlash.Core;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for the Customer entity
    /// </summary>
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly IDataContextFactory dataContextFactory;
        public CustomerController(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Get list of Customers
        /// </summary>
        /// <returns>Customer index list view</returns>
        public ActionResult Index()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                //Eager loading Customer
                var customerQuery = (from x in context.Customers select x)
                                    .Include(x => x.Country)
                                    .OrderBy(x => x.Name);

                var viewModel = new CustomerIndexViewModel(context.GetUser(HttpContext.User.Identity), customerQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Create a single Customer
        /// </summary>
        /// <returns>Create Customer view</returns>
        public ActionResult Create()
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                CheckAuthorizedToCreate(context);

                var countryQuery = from x in context.Countries select x;

                var viewModel = new CustomerCreateViewModel(countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save created Customer into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Created CustomerCreateViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(CustomerCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    CheckAuthorizedToCreate(context);

                    var customer = viewModel.ToEntity(null);
                    context.Customers.Add(customer);
                    context.UserCustomerRights.Add(new UserCustomerRight
                    {
                        RightObject = customer,
                        RightId = EditEntityMembers.Id,
                        User = context.GetUser(User.Identity)
                    });
                    
                    context.SaveChanges();
                    Flash.Success("The customer was successfully created.");
                }
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        private void CheckAuthorizedToCreate(IDataContextByUser context)
        {
            var user = context.GetUser(HttpContext.User.Identity);

            if (!user.IsVendorAdmin && !user.IsSystemAdmin)
                throw new UnauthorizedAccessException("Only system or vendor admins may create customers.");
        }

        /// <summary>
        /// Edit a single Customer
        /// </summary>
        /// <param name="key">Key of the customer to edit</param>
        /// <returns>Edit Customer view</returns>
        public ActionResult Edit(Guid key)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var customer = (from x in context.Customers where x.ObjectId == key select x).SingleOrDefault();
                
                if (customer == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                var countryQuery = from x in context.Countries select x;

                var viewModel = new CustomerEditViewModel(customer, countryQuery.ToList());

                return View(viewModel);
            }
        }

        /// <summary>
        /// Save edited Customer into db and redirect to index
        /// </summary>
        /// <param name="viewModel">Edited CustomerEditViewModel</param>
        /// <returns>Redirectaction to index if successfull</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = dataContextFactory.CreateByUser())
                {
                    var customer = (from x in context.Customers where x.ObjectId == viewModel.Customer.ObjectId select x).SingleOrDefault();

                    if (customer == null)
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                    viewModel.ToEntity(customer);

                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }
    }
}
