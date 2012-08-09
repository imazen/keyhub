using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Model
{
    // TODO: Finish this entity
    /// <summary>
    /// Transaction for storing transaction information
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Indentifier for the Transaction entity.
        /// </summary>
        [Key]
        public int TransactionId { get; set; }

    }
}