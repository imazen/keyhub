using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.License
{
    /// <summary>
    /// Viewmodel for a single License 
    /// </summary>
    public class LicenseViewModel : BaseViewModel<Model.License>
    {
        public LicenseViewModel() : base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="License">License that this viewmodel represents</param>
        public LicenseViewModel(Model.License license)
            : this()
        {
            this.ObjectId = license.ObjectId;
            this.SkuId = license.SkuId;
            this.PurchasingCustomerId = license.PurchasingCustomerId;
            this.OwnerName = license.OwnerName;
            this.OwningCustomerId = license.OwningCustomerId;
            this.LicenseIssued = license.LicenseIssued;
            this.LicenseExpires = license.LicenseExpires;
        }

        /// <summary>
        /// Convert back to License instance
        /// </summary>
        /// <param name="original">Original License. If Null a new instance is created.</param>
        /// <returns>License containing viewmodel data </returns>
        public override Model.License ToEntity(Model.License original)
        {
            Model.License current = original ?? new Model.License();

            current.ObjectId = this.ObjectId;
            current.SkuId = this.SkuId;
            current.PurchasingCustomerId = this.PurchasingCustomerId;
            current.OwnerName = this.OwnerName;
            current.OwningCustomerId = this.OwningCustomerId;
            current.LicenseIssued = this.LicenseIssued;
            current.LicenseExpires = this.LicenseExpires;

            return current;
        }

        /// <summary>
        /// The primary key and identifier for this customer
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The SKU bought with this license
        /// </summary>
        [Required]
        [DisplayName("SKU")]
        public Guid SkuId { get; set; }

        /// <summary>
        /// The Customer that purchased this license
        /// </summary>
        [Required]
        [DisplayName("Purchaser")]
        public Guid PurchasingCustomerId { get; set; }

        /// <summary>
        /// The original name of the purchasing entity that bought this license
        /// </summary>
        [Required]
        [StringLength(256)]
        [DisplayName("Original owner name")]
        public string OwnerName { get; set; }

        /// <summary>
        /// The Customer that ownes this license
        /// </summary>
        /// <remarks>Can be empty if license is unassigned?</remarks>
        [DisplayName("Owner")]
        public Guid OwningCustomerId { get; set; }

        /// <summary>
        /// The date this license has been issued
        /// </summary>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")] 
        public DateTime LicenseIssued { get; set; }

        /// <summary>
        /// The date this license will expire
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")] 
        public DateTime? LicenseExpires { get; set; }
    }
}