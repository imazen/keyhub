using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyHub.Web.ViewModels.CustomerAppIssue
{
    /// <summary>
    /// Viewmodel for index list of Customers
    /// </summary>
    public class CustomerAppIssueIndexViewModel : RedirectUrlModel
    {
        public CustomerAppIssueIndexViewModel() : base()
        { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="customerAppIssueList">List of CustomerAppIssue entities</param>
        public CustomerAppIssueIndexViewModel(IEnumerable<Model.CustomerAppIssue> customerAppIssueList)
            : base()
        {
            CustomerAppIssues = new List<CustomerAppIssueViewModel>(
                    customerAppIssueList.Select(x => new CustomerAppIssueViewModel(x))
                );
        }

        /// <summary>
        /// List of customers
        /// </summary>
        public List<CustomerAppIssueViewModel> CustomerAppIssues { get; set; }
    }
}