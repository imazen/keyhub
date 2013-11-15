using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.PrivateKey
{
    /// <summary>
    /// Viewmodel for a single privateKey 
    /// </summary>
    public class PrivateKeyViewModel : RedirectUrlModel
    {
        public PrivateKeyViewModel():base(){ }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="privateKey">PrivateKey that this viewmodel represents</param>
        public PrivateKeyViewModel(Model.PrivateKey privateKey):this()
        {
            this.PrivateKeyId = privateKey.PrivateKeyId;
            this.VendorId = privateKey.VendorId;
            this.DisplayName = privateKey.DisplayName;
            this.PublicKeyXmlString = privateKey.GetPublicKeyXmlString();
        }

        /// <summary>
        /// Convert back to PrivateKey instance
        /// </summary>
        /// <param name="original">Original PrivateKey. If Null a new instance is created.</param>
        /// <returns>PrivateKey containing viewmodel data </returns>
        public Model.PrivateKey ToEntity(Model.PrivateKey original)
        {
            if (original == null)
            {
                original = new Model.PrivateKey();
                original.SetKeyBytes();
            }
            Model.PrivateKey current = original;

            current.PrivateKeyId = this.PrivateKeyId;
            current.VendorId = this.VendorId;
            current.DisplayName = this.DisplayName;

            return current;
        }

        /// <summary>
        /// Indentifier for the PrivateKey.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid PrivateKeyId { get; set; }

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

        [Required]
        public string PublicKeyXmlString { get; set; }
    }
}