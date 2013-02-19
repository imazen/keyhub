using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Model;

namespace KeyHub.Web.ViewModels.Transaction
{
    /// <summary>
    /// Viewmodel for a single transaction 
    /// </summary>
    public class TransactionViewModel : BaseViewModel<Model.Transaction>
    {
        public TransactionViewModel():base(){ }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="transaction">Transaction that this viewmodel represents</param>
        public TransactionViewModel(Model.Transaction transaction)
            : this()
        {
            this.TransactionId = transaction.TransactionId;
            this.Status = transaction.Status;
            this.CreatedDateTime = transaction.CreatedDateTime;
        }

        /// <summary>
        /// Convert back to Transaction instance
        /// </summary>
        /// <param name="original">Original Transaction. If Null a new instance is created.</param>
        /// <returns>Transaction containing viewmodel data </returns>
        public override Model.Transaction ToEntity(Model.Transaction original)
        {
            Model.Transaction current = original ?? new Model.Transaction();

            //Update from viewmodel, never update createdDateTime
            current.TransactionId = this.TransactionId;
            current.Status = this.Status;
            
            return current;
        }

        /// <summary>
        /// Indentifier for the Transaction entity.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int TransactionId { get; set; }

        /// <summary>
        /// Status of the transaction
        /// </summary>
        [Required]
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// Date the transaction was created on
        /// </summary>
        [DisplayName("Transaction date/time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy HH:mm:ss}")] 
        public DateTime CreatedDateTime { get; set; }
    }
}