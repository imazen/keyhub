using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

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
        public CustomerAppIndexViewItem(Model.CustomerApp customerApp, IEnumerable<Model.License> customerAppLicenses)
            : base(customerApp)
        {
            LicenseSummary = BuildFeatureSummary(customerAppLicenses);
        }

        /// <summary>
        /// Builds a summary of licenses assigned to an CustomerApp
        /// </summary>
        /// <param name="customerAppFeatures">List of assigned features</param>
        /// <returns>Summary</returns>
        private string BuildFeatureSummary(IEnumerable<Model.License> customerAppLicenses)
        {
            const int MAXLINES = 3;
            const string LINEFEED = ", ";
            string summary = "";

            var filteredLicenses = customerAppLicenses.Take(MAXLINES);

            if (filteredLicenses.Count() > 0)
            {
                summary = string.Join(LINEFEED, filteredLicenses.Select(x => x.Sku.SkuCode));
                if (customerAppLicenses.Count() > MAXLINES)
                    summary += string.Format(" and {0} more...", customerAppLicenses.Count() - MAXLINES);
            }
            else
                summary = "No license assigned";

            return summary;
        }

        /// <summary>
        /// Summary of assigned Licenses
        /// </summary>
        [DisplayName("Licenses")]
        public string LicenseSummary { get; set; }
    }
}