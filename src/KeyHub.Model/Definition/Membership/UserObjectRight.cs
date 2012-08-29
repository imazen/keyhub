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
    /// Defines the base model for all right join tables (currently: vendor rights, customer rights and license rights)
    /// </summary>
    public abstract class UserObjectRight
    {
        /// <summary>
        /// The UserId associated with this right entry
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }

        /// <summary>
        /// The ObjectId this right entry refers to
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The RightId this right entry refers to
        /// </summary>
        [Key]
        [Column(Order = 3)]
        public Guid RightId { get; set; }

        /// <summary>
        /// The user associated with this right entry
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        /// <summary>
        /// The object this right entry refers to
        /// </summary>
        [ForeignKey("ObjectId")]
        public virtual RightObject RightObject { get; set; }

        /// <summary>
        /// The type of right this entry is made for
        /// </summary>
        [ForeignKey("RightId")]
        public virtual Right Right { get; set; }
    }
}