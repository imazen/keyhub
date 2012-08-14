using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.PrivateKey
{
    /// <summary>
    /// Viewmodel for creation of a PrivateKey
    /// </summary>
    public class PrivateKeyCreateViewModel : BaseViewModel<Model.PrivateKey>
    {
        public PrivateKeyCreateViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="vendor">Vendor of this PrivateKey</param>
        public PrivateKeyCreateViewModel(Model.Vendor vendor)
        {
            Model.PrivateKey privateKey = new Model.PrivateKey();
            privateKey.SetKeyBytes();

            PrivateKey = new PrivateKeyViewModel(privateKey);
            PrivateKey.VendorId = vendor.ObjectId;
        }
        
        /// <summary>
        /// Edited privateKey
        /// </summary>
        public PrivateKeyViewModel PrivateKey { get; set; }

        /// <summary>
        /// Convert back to PrivateKey instance
        /// </summary>
        /// <returns>New privateKey containing viewmodel data </returns>
        public override Model.PrivateKey ToEntity(Model.PrivateKey original)
        {
            return PrivateKey.ToEntity(null);
        }
    }
}