using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.CustomerApp
{
    /// <summary>
    /// Viewmodel for a single CustomerApp 
    /// </summary>
    public class CustomerAppViewModel : BaseViewModel<Model.CustomerApp>
    {
        public CustomerAppViewModel() : base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="CustomerApp">CustomerApp that this viewmodel represents</param>
        public CustomerAppViewModel(Model.CustomerApp customerApp)
            : this()
        {
            this.CustomerAppId = customerApp.CustomerAppId;
            this.ApplicationName = customerApp.ApplicationName;
        }

        /// <summary>
        /// Convert back to CustomerApp instance
        /// </summary>
        /// <param name="original">Original CustomerApp. If Null a new instance is created.</param>
        /// <returns>CustomerApp containing viewmodel data </returns>
        public override Model.CustomerApp ToEntity(Model.CustomerApp original)
        {
            Model.CustomerApp current = original ?? new Model.CustomerApp();

            current.CustomerAppId = this.CustomerAppId;
            current.ApplicationName = this.ApplicationName;

            return current;
        }

        /// <summary>
        /// Indentifier for the CustomerApp entity
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid CustomerAppId { get; set; }

        /// <summary>
        /// The name of the customer application
        /// </summary>
        [Required]
        [StringLength(256)]
        public string ApplicationName { get; set; }
    }
}