using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the basic object for KeyHub to indicate this object needs rights
    /// </summary>
    public abstract class RightObject
    {
        /// <summary>
        /// The primary key and identifier for this object
        /// </summary>
        [Key]
        public Guid ObjectId { get; set; }
    }
}