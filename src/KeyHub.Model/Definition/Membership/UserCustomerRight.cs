using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the join table between user-rights and Customers
    /// </summary>
    public class UserCustomerRight : UserObjectRight
    {
        /// <summary>
        /// The Customer associated with this right entry
        /// </summary>
        [ForeignKey("ObjectId")]
        public virtual Customer Customer { get; set; }
    }
}