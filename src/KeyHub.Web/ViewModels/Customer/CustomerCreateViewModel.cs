using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.Customer
{
    /// <summary>
    /// Viewmodel for creating a Customer
    /// </summary>
    public class CustomerCreateViewModel : BaseViewModel<Model.Customer>
    {
        public CustomerCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="countries">List of countries to select</param>
        public CustomerCreateViewModel(List<Model.Country> countries)
        {
            Customer = new CustomerViewModel(new Model.Customer());

            CountryList = countries.ToSelectList(x => x.CountryCode, x => x.CountryName);
        }

        /// <summary>
        /// Creating Customer
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
        public override Model.Customer ToEntity(Model.Customer original)
        {
            return Customer.ToEntity(null);
        }
    }
}