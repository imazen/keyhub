using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Web.ViewModels.User;

namespace KeyHub.Web.ViewModels.Customer
{
    /// <summary>
    /// Viewmodel for index list of Customers
    /// </summary>
    public class CustomerIndexViewModel : BaseViewModel<Model.Customer>
    {
        public CustomerIndexViewModel() : base()
        { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="currentUser">Current user</param>
        /// <param name="customerList">List of Customer entities</param>
        public CustomerIndexViewModel(Model.User currentUser, IEnumerable<Model.Customer> customerList)
            : base()
        {
            CurrentUser = new CurrentUserViewModel(currentUser);

            Customers = new List<CustomerIndexViewItem>(
                    customerList.Select(x => new CustomerIndexViewItem(x, x.Country))
                );
        }

        /// <summary>
        /// List of customers
        /// </summary>
        public List<CustomerIndexViewItem> Customers { get; set; }

        /// <summary>
        /// Gets the current user viewmodel
        /// </summary>
        public CurrentUserViewModel CurrentUser { get; private set; }
    }

    /// <summary>
    /// CustomerViewModel extension that includes Country name
    /// </summary>
    public class CustomerIndexViewItem : CustomerViewModel
    {
        public CustomerIndexViewItem(Model.Customer customer, Model.Country country)
            : base(customer)
        {
            CountryName = country.CountryName;
        }

        /// <summary>
        /// CountryName of the Country
        /// </summary>
        [DisplayName("Country")]
        public string CountryName { get; set; }
    }
}