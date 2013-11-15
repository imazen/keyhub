using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.License
{
    /// <summary>
    /// Viewmodel for editing an License
    /// </summary>
    public class LicenseEditViewModel : RedirectUrlModel
    {
        public LicenseEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="license">License entity</param>
        /// <param name="skus">List of SKUs to select</param>
        /// <param name="customers">List of customers to select</param>
        public LicenseEditViewModel(Model.License license, List<Model.SKU> skus, List<Model.Customer> customers)
        {
            this.License = new LicenseViewModel(license);

            this.SKUList = skus.ToSelectList(x => x.SkuId, x => x.SkuCode);
            this.CustomerList = customers.ToSelectList(x => x.ObjectId, x => x.Name);
        }

        /// <summary>
        /// Edited vendor
        /// </summary>
        public LicenseViewModel License { get; set; }

        /// <summary>
        /// List of SKUs to select
        /// </summary>
        public SelectList SKUList { get; set; }

        /// <summary>
        /// List of customers to select
        /// </summary>
        public SelectList CustomerList { get; set; }

        /// <summary>
        /// Convert back to License instance
        /// </summary>
        /// <param name="original">Original License. If Null a new instance is created.</param>
        /// <returns>License containing viewmodel data </returns>
        public Model.License ToEntity(Model.License original)
        {
            return License.ToEntity(original);
        }
    }
}