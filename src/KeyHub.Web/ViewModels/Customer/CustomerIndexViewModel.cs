using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.Customer
{
    /// <summary>
    /// Viewmodel for index list of Customers
    /// </summary>
    public class CustomerIndexViewModel : BaseViewModel<Model.Customer>
    {
        public CustomerIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="customerList">List of Customer entities</param>
        public CustomerIndexViewModel(List<Model.Customer> customerList)
            : this()
        {
            Customers = new List<CustomerIndexViewItem>(
                    customerList.Select(x => new CustomerIndexViewItem(x, x.Country))
                );
        }

        /// <summary>
        /// List of customers
        /// </summary>
        public List<CustomerIndexViewItem> Customers { get; set; }

        /// <summary>
        /// Convert back to Customer instance
        /// </summary>
        /// <param name="original">Original Customer. If Null a new instance is created.</param>
        /// <returns>Customer containing viewmodel data </returns>
        public override Model.Customer ToEntity(Model.Customer original)
        {
            throw new NotImplementedException();
        }
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