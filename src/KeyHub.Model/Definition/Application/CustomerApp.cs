using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Provides registration of client application (the developers who use it)
    /// </summary>
    public partial class CustomerApp
    {
        public CustomerApp()
        {
            LicenseCustomerApps = new List<LicenseCustomerApp>();
            CustomerAppKeys = new List<CustomerAppKey>();
        }

        /// <summary>
        /// Indentifier for the CustomerApp entity
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid CustomerAppId { get; set; }

        /// <summary>
        /// The name of the customer application
        /// </summary>
        [Required]
        [StringLength(256)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// A list of LicenseCustomerApp (<see cref="License"/>) this application is associated with
        /// </summary>
        public virtual ICollection<LicenseCustomerApp> LicenseCustomerApps { get; set; }

        /// <summary>
        /// A list of CustomerAppKeys (<see cref="CustomerAppKey"/>) of this application
        /// </summary>
        public virtual ICollection<CustomerAppKey> CustomerAppKeys { get; set; }
    }
}