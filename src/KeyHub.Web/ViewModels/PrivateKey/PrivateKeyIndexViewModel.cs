using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KeyHub.Web.ViewModels.PrivateKey
{
    /// <summary>
    /// Viewmodel for index list of a PrivateKey
    /// </summary>
    public class PrivateKeyIndexViewModel : BaseViewModel<Model.PrivateKey>
    {
        public PrivateKeyIndexViewModel() : base() { }

        /// <summary>
        /// Construct the viewmodel
        /// </summary>
        /// <param name="privateKeyList">List of privateKey entities</param>
        /// <param name="parentVendor">Vendor entity that ownes these private keys</param>
        public PrivateKeyIndexViewModel(List<Model.PrivateKey> privateKeyList, Model.Vendor parentVendor):this()
        {
            PrivateKeys = new List<PrivateKeyViewModel>();
            foreach (Model.PrivateKey entity in privateKeyList)
            {
                PrivateKeys.Add(new PrivateKeyViewModel(entity));
            }

            ParentVendorGUID = parentVendor.ObjectId;
        }

        /// <summary>
        /// List of privateKeys
        /// </summary>
        public List<PrivateKeyViewModel> PrivateKeys { get; set; }

        /// <summary>
        /// GUID of the parent vendor
        /// </summary>
        public Guid ParentVendorGUID { get; set; }

        /// <summary>
        /// Convert back to PrivateKey instance
        /// </summary>
        /// <param name="original">Original PrivateKey. If Null a new instance is created.</param>
        /// <returns>PrivateKey containing viewmodel data </returns>
        public override Model.PrivateKey ToEntity(Model.PrivateKey original)
        {
            throw new NotImplementedException();
        }
    }
}