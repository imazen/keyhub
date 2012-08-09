using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    /// <summary>
    /// Defines the base object for KeyHub to indicate this object needs rights.
    /// This class will not convert into a table. It's used to supply all rights object with
    /// the same PrimaryKey.
    /// </summary>
    public abstract class RightObject
    {
        /// <summary>
        /// The primary key and identifier for this object
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid ObjectId { get; set; }
    }
}