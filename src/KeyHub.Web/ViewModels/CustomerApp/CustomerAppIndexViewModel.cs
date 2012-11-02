using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.CustomerApp
{
    /// <summary>
    /// Viewmodel for index list of CustomerApps
    /// </summary>
    public class CustomerAppIndexViewModel : BaseViewModel<Model.CustomerApp>
    {
        public CustomerAppIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="CustomerAppList">List of CustomerApp entities</param>
        public CustomerAppIndexViewModel(List<Model.CustomerApp> customerAppList)
            : this()
        {
            CustomerApps = new List<CustomerAppIndexViewItem>();
            foreach (Model.CustomerApp entity in customerAppList)
            {
                CustomerApps.Add(new CustomerAppIndexViewItem(entity, 
                    entity.LicenseCustomerApps.Select(x=> x.License).OrderBy(x => x.LicenseIssued)));
            }
        }

        /// <summary>
        /// List of CustomerApps
        /// </summary>
        public List<CustomerAppIndexViewItem> CustomerApps { get; set; }

        /// <summary>
        /// Convert back to CustomerApp instance
        /// </summary>
        /// <param name="original">Original CustomerApp. If Null a new instance is created.</param>
        /// <returns>CustomerApp containing viewmodel data </returns>
        public override Model.CustomerApp ToEntity(Model.CustomerApp original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// CustomerAppViewModel extension that includes Vendor name, Primary key name, summary of features
    /// </summary>
    public class CustomerAppIndexViewItem : CustomerAppViewModel
    {
        private string example = "<resizer><licenses><auto appId=\"{0}\"/><licenses></resizer>";

        public CustomerAppIndexViewItem(Model.CustomerApp customerApp, IEnumerable<Model.License> customerAppLicenses)
            : base(customerApp)
        {
            LicenseSummary = customerAppLicenses.ToSummary(x => x.Sku.SkuCode, 3, ", ");
            if (customerApp.CustomerAppKeys.Any())
                ActiveCustomerAppKey = customerApp.CustomerAppKeys.Last().AppKey;

            WebConfigExample = string.Format(example, ActiveCustomerAppKey);
        }

        /// <summary>
        /// Summary of assigned Licenses
        /// </summary>
        [DisplayName("Licensed skus")]
        public string LicenseSummary { get; set; }

        /// <summary>
        /// Summary of assigned Licenses
        /// </summary>
        [DisplayName("Customer App Key")]
        public Guid ActiveCustomerAppKey { get; set; }

        /// <summary>
        /// Summary of assigned Licenses
        /// </summary>
        [DisplayName("Web.config example")]
        public string WebConfigExample { get; set; }
    }
}