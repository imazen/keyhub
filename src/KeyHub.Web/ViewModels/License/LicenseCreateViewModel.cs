using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.License
{
    /// <summary>
    /// Viewmodel for creating an License
    /// </summary>
    public class LicenseCreateViewModel : RedirectUrlModel
    {
        public LicenseCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="skus">List of SKUs to select</param>
        /// <param name="customers">List of customers to select</param>
        public LicenseCreateViewModel(List<Model.SKU> skus, List<Model.Customer> customers)
        {
            License = new LicenseViewModel(new Model.License());

            SKUList = skus.ToSelectList(x => x.SkuId, x => x.SkuCode);
            CustomerList = customers.ToSelectList(x => x.ObjectId, x => x.Name);
        }

        /// <summary>
        /// Creating License
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
            return License.ToEntity(null);
        }
    }
}