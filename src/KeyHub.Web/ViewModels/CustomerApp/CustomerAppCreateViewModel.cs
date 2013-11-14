using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Data;

namespace KeyHub.Web.ViewModels.CustomerApp
{
    /// <summary>
    /// Viewmodel for creating an CustomerApp
    /// </summary>
    public class CustomerAppCreateViewModel
    {
        [Required]
        [StringLength(256)]
        public string ApplicationName { get; set; }

        public List<Guid> SelectedLicenseGUIDs { get; set; }

        /// <summary>
        /// List of licenses to select
        /// </summary>
        public IEnumerable<SelectListItem> LicenseList { get; set; }
    }
}