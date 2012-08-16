using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KeyHub.Runtime;

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
        }

        /// <summary>
        /// Convert back to Transaction instance
        /// </summary>
        /// <param name="original">Original Transaction. If Null a new instance is created.</param>
        /// <returns>Transaction containing viewmodel data </returns>
        public override Model.Transaction ToEntity(Model.Transaction original)
        {
            Model.Transaction current = original ?? new Model.Transaction();

            current.TransactionId = this.TransactionId;

            return current;
        }

        /// <summary>
        /// Indentifier for the Transaction entity.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int TransactionId { get; set; }
    }
}