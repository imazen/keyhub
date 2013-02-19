using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;
using KeyHub.Runtime;
using KeyHub.Web.ViewModels.CustomerAppIssue;
using KeyHub.Web.ViewModels.License;

namespace KeyHub.Web.Controllers
{
    [Authorize]
    public class CustomerAppIssueController : Controller
    {
        private readonly IDataContextFactory dataContextFactory;
        public CustomerAppIssueController(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Show a list of CustomerAppIssues by CustomerApp
        /// </summary>
        /// <param name="customerAppId">CustomerApp to show Issues for</param>
        /// <returns></returns>
        public ActionResult Index(Guid customerAppId)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var customerAppIssues =
                    (from x in context.CustomerAppIssues where x.CustomerAppId == customerAppId select x);

                var viewModel = new CustomerAppIssueIndexViewModel(customerAppIssues);
                
                return View(viewModel);
            }
        }

        /// <summary>
        /// Get partial list of customer app issues by customer app
        /// </summary>
        /// <param name="customerAppId">CustomerAppId to show customer apps issues for</param>
        /// <returns>CustomerAppIssue index list view</returns>
        public ActionResult IndexPartial(Guid customerAppId)
        {
            using (var context = dataContextFactory.CreateByUser())
            {
                var customerAppIssues =
                    (from x in context.CustomerAppIssues where x.CustomerAppId == customerAppId select x);

                var viewModel = new CustomerAppIssueIndexViewModel(customerAppIssues);

                return PartialView(viewModel);
            }
        }

        /// <summary>
        /// Remove a single CustomerAppIssue
        /// </summary>
        /// <param name="key">Key of CustomerAppIssue to remove</param>
        /// <param name="customerAppKey">Customer app to return to</param>
        /// <returns></returns>
        public ActionResult Remove(string key, Guid customerAppKey)
        {
            int decryptedKey = Common.Utils.SafeConvert.ToInt(key.DecryptUrl(), -1);

            using (var context = dataContextFactory.CreateByUser())
            {
                context.CustomerAppIssues.Remove(x => x.CustomerAppIssueId == decryptedKey);
                context.SaveChanges();
            }

            return RedirectToAction("Details", "CustomerApp", new { key = customerAppKey });
        }

        /// <summary>
        /// Details of a single transaction
        /// </summary>
        /// <param name="key">Id of the transaction to show</param>
        /// <returns>Transaction details view</returns>
        public ActionResult Details(string key)
        {
            int decryptedKey = Common.Utils.SafeConvert.ToInt(key.DecryptUrl(), -1);

            using (var context = dataContextFactory.CreateByUser())
            {
                var customerAppQuery = (from x in context.CustomerAppIssues where x.CustomerAppIssueId == decryptedKey select x);

                if (customerAppQuery.FirstOrDefault() == null)
                    throw new EntityNotFoundException("CustomerAppIssue could not be resolved!");

                var viewModel = new CustomerAppIssueViewModel(customerAppQuery.FirstOrDefault());

                return View(viewModel);
            }
        }
    }
}
