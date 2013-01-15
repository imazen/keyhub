using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;

namespace KeyHub.Web.ViewModels.Vendor
{
    /// <summary>
    /// Viewmodel for a single vendor 
    /// </summary>
    public class VendorViewModel : BaseViewModel<Model.Vendor>
    {
        public VendorViewModel():base(){ }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="vendor">Vendor that this viewmodel represents</param>
        public VendorViewModel(Model.Vendor vendor):this()
        {
            this.ObjectId = vendor.ObjectId;
            this.Name = vendor.Name;
            this.Street = vendor.Street;
            this.PostalCode = vendor.PostalCode;
            this.City = vendor.City;
            this.Region = vendor.Region;
            this.CountryCode = vendor.CountryCode;
        }

        /// <summary>
        /// Convert back to Vendor instance
        /// </summary>
        /// <param name="original">Original Vendor. If Null a new instance is created.</param>
        /// <returns>Vendor containing viewmodel data </returns>
        public override Model.Vendor ToEntity(Model.Vendor original)
        {
            Model.Vendor current = original ?? new Model.Vendor();

            current.Name = this.Name;
            current.Street = this.Street;
            current.PostalCode = this.PostalCode;
            current.City = this.City;
            current.Region = this.Region;
            current.CountryCode = this.CountryCode;

            return current;
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        [DisplayName("Vendor Id")]
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The public name for this vendor
        /// </summary>
        [Required]
        [StringLength(512)]
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The street this vendor is located at.
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Street { get; set; }

        /// <summary>
        /// The postal code this vendor is located in.
        /// </summary>
        [Required]
        [StringLength(24)]
        [DisplayName("Postal code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// The city this vendor is located in.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// The region this vendor is located in (for Americans it's the state, for Dutch it's province)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Region { get; set; }

        /// <summary>
        /// CountryCode of the customer.
        /// </summary>
        [Required(ErrorMessage="Please select a country")]
        [StringLength(12)]
        [DisplayName("Country")]
        public string CountryCode { get; set; }
    }
}