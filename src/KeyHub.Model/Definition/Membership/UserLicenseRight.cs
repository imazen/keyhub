using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the join table between user-rights and Licenses
    /// </summary>
    public class UserLicenseRight : UserObjectRight
    {
        /// <summary>
        /// The License associated with this right entry
        /// </summary>
        [ForeignKey("ObjectId")]
        public virtual License License { get; set; }
    }
}