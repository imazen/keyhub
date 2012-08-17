﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.License
{
    /// <summary>
    /// Viewmodel for index list of Licenses
    /// </summary>
    public class LicenseIndexViewModel : BaseViewModel<Model.License>
    {
        public LicenseIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="licenseList">List of License entities</param>
        public LicenseIndexViewModel(List<Model.License> licenseList)
            : this()
        {
            Licenses = new List<LicenseIndexViewItem>(
                    licenseList.Select(x => new LicenseIndexViewItem(x, x.OwningCustomer, x.Sku))
                );
        }

        /// <summary>
        /// List of licenses
        /// </summary>
        public List<LicenseIndexViewItem> Licenses { get; set; }

        /// <summary>
        /// Convert back to License instance
        /// </summary>
        /// <param name="original">Original License. If Null a new instance is created.</param>
        /// <returns>License containing viewmodel data </returns>
        public override Model.License ToEntity(Model.License original)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// LicenseViewModel extension that includes Owning Customer name
    /// </summary>
    public class LicenseIndexViewItem : LicenseViewModel
    {
        public LicenseIndexViewItem(Model.License license, Model.Customer owningCustomer, Model.SKU sku)
            : base(license)
        {
            OwningCustomerName = (owningCustomer != null) ? owningCustomer.Name : "Not owned";
            SKUName = sku.SkuCode;
        }

        /// <summary>
        /// Name of the customer that ownes the license
        /// </summary>
        [DisplayName("Owner")]
        public string OwningCustomerName { get; set; }

        /// <summary>
        /// Name of the SKU bought with this license
        /// </summary>
        [DisplayName("SKU")]
        public string SKUName { get; set; }
    }
}