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
    public class PrivateKeyEditViewModel : RedirectUrlModel
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
    }
}