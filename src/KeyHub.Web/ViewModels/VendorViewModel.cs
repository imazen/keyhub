using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;

namespace KeyHub.Web.ViewModels
{

    /// <summary>
    /// Generic converter to create a bridge between domain model and view model
    /// </summary>
    /// <typeparam name="TContext">Type of the DbContext</typeparam>
    /// <typeparam name="TEntity">Type of the entity to return</typeparam>
    /// <typeparam name="TPrimaryKey">Type of the primary key</typeparam>
    public abstract class ViewModelConverter<TContext, TEntity, TPrimaryKey> 
        where TContext : DbContext where TEntity : class
    {
        public abstract TEntity ToEntity(TEntity original);

        public abstract void Build(TPrimaryKey key, TContext context);
    }


    public class VendorViewModel : ViewModelConverter<DataContext, Model.Vendor, Guid>
    {
        public VendorData Vendor { get; set; }
        public SelectList CountryList { get; set; }

        public override Model.Vendor ToEntity(Model.Vendor original)
        {
            Model.Vendor currentVendor = original ?? new Model.Vendor();

            // dingen doen met currentvendor -> vullen vanuit ViewModel

            return original;
        }

        public override void Build(Guid key, DataContext context)
        {
            
            
            throw new NotImplementedException();
        }
    }

    public class VendorData
    {
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The public name for this vendor/organisation
        /// </summary>
        [Required]
        [StringLength(512)]
        [DisplayName("Organisation name")]
        public string OrganisationName { get; set; }

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
        [Required]
        [StringLength(12)]
        public string CountryCode { get; set; }
    }
}