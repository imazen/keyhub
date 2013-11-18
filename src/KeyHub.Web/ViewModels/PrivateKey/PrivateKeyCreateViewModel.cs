using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.PrivateKey
{
    /// <summary>
    /// Viewmodel for creation of a PrivateKey
    /// </summary>
    public class PrivateKeyCreateViewModel
    {
        public PrivateKeyCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor of this PrivateKey</param>
        public PrivateKeyCreateViewModel(Model.Vendor vendor)
        {
            VendorId = vendor.ObjectId;
        }

        /// <summary>
        /// The vendor this key is owned by.
        /// </summary>
        [Required]
        public Guid VendorId { get; set; }

        /// <summary>
        /// Display name for this private key
        /// </summary>
        [Required]
        [StringLength(256)]
        public string DisplayName { get; set; }
    }
}