using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Represents a right-type. These types will be used to allow users to specific parts of KeyHub.
    /// Each right has a unique GUID to identity itself. These GUIDS are system defined, and cannot be changed.
    /// </summary>
    public class Right
    {
        /// <summary>
        /// The unique right type GUID to identity what kind of right this entry is
        /// </summary>
        [Key]
        public Guid RightId { get; set; }

        /// <summary>
        /// Display name for human readability
        /// </summary>
        [StringLength(256)]
        public string DisplayName { get; set; }
    }
}