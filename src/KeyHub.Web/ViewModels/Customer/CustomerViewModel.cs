using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.Customer
{
    /// <summary>
    /// Viewmodel for a single Customer 
    /// </summary>
    public class CustomerViewModel : RedirectUrlModel
    {
        public CustomerViewModel() : base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="Customer">Customer that this viewmodel represents</param>
        public CustomerViewModel(Model.Customer customer)
            : base()
        {
            ObjectId = customer.ObjectId;
            Name = customer.Name;
            Department = customer.Department;
            Street = customer.Street;
            PostalCode = customer.PostalCode;
            City = customer.City;
            Region = customer.Region;
            CountryCode = customer.CountryCode;
        }

        /// <summary>
        /// Convert back to Customer instance
        /// </summary>
        /// <param name="target">Original Customer. If Null a new instance is created.</param>
        /// <returns>Customer containing viewmodel data </returns>
        public Model.Customer ToEntity(Model.Customer target)
        {
            target.ObjectId = this.ObjectId;
            target.Name = this.Name;
            target.Department = this.Department;
            target.Street = this.Street;
            target.PostalCode = this.PostalCode;
            target.City = this.City;
            target.Region = this.Region;
            target.CountryCode = this.CountryCode;

            return target;
        }

        /// <summary>
        /// The primary key and identifier for this customer
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The name of the customer (single line)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// The department this Customer is buying for/from
        /// </summary>
        [StringLength(512)]
        public string Department { get; set; }

        /// <summary>
        /// The street this customer is located at.
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Street { get; set; }

        /// <summary>
        /// The postal code this customer is located in.
        /// </summary>
        [Required]
        [StringLength(24)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The city this customer is located in.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// The region this customer is located in (for Americans it's the state, for Dutch it's province)
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Region { get; set; }

        /// <summary>
        /// CountryCode of the customer.
        /// </summary>
        [Required]
        [StringLength(12)]
        public string CountryCode { get; set; }
    }
}