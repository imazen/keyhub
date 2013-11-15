using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Customer
{
    /// <summary>
    /// Viewmodel for editing an Customer
    /// </summary>
    public class CustomerEditViewModel : RedirectUrlModel
    {
        public CustomerEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="customer">Customer entity</param>
        /// <param name="countries">List of countries to select</param>
        public CustomerEditViewModel(Model.Customer customer, List<Model.Country> countries)
            : base()
        {
            Customer = new CustomerViewModel(customer);

            CountryList = countries.ToSelectList(x => x.CountryCode, x => x.CountryName);
        }

        /// <summary>
        /// Edited vendor
        /// </summary>
        public CustomerViewModel Customer { get; set; }

        /// <summary>
        /// List of countries to select
        /// </summary>
        public SelectList CountryList { get; set; }

        /// <summary>
        /// Convert back to Customer instance
        /// </summary>
        /// <param name="original">Original Customer. If Null a new instance is created.</param>
        /// <returns>Customer containing viewmodel data </returns>
        public Model.Customer ToEntity(Model.Customer original)
        {
            return Customer.ToEntity(original);
        }
    }
}