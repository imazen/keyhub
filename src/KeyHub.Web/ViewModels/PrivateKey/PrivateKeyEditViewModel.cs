using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.PrivateKey
{
    /// <summary>
    /// Viewmodel for editing a PrivateKey
    /// </summary>
    public class PrivateKeyEditViewModel : BaseViewModel<Model.PrivateKey>
    {
        public PrivateKeyEditViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="privateKey">PrivateKey entity</param>
        public PrivateKeyEditViewModel(Model.PrivateKey privateKey)
        {
            PrivateKey = new PrivateKeyViewModel(privateKey);
        }
        
        /// <summary>
        /// Edited privateKey
        /// </summary>
        public PrivateKeyViewModel PrivateKey { get; set; }

        /// <summary>
        /// Convert back to PrivateKey instance
        /// </summary>
        /// <param name="original">Original PrivateKey. If Null a new instance is created.</param>
        /// <returns>PrivateKey containing viewmodel data </returns>
        public Model.PrivateKey ToEntity(Model.PrivateKey original)
        {
            return PrivateKey.ToEntity(original);
        }
    }
}