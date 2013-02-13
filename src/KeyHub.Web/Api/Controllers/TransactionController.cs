using System;
using System.Threading.Tasks;
using System.Web.Http;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Common.Utils;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Web.Api.Controllers.LicenseValidation;
using KeyHub.Web.Api.Controllers.Transaction;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// REST controller for creating new transactions.
    /// </summary>
    public class TransactionController : BaseTransactionController
    {
        public TransactionController(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        /// <summary>
        /// Transaction post
        /// </summary>
        /// <param name="transaction">Transaction to create</param>
        /// <returns>TransactionResult containing success status and optional errormessage</returns>
        /// <example>
        ///     POST http://localhost:63436/api/transaction/ HTTP/1.2
        ///     User-Agent: Fiddler
        ///     Host: localhost:63436
        ///     Content-Length: 185
        ///     Content-Type: application/xml
        ///     <TransactionRequest PurchaserName="Steven Somer" PurchaserEmail="steven@lucrasoft.nl">
        ///         <PurchasedSku>{guid}</PurchasedSku>
        ///     </TransactionRequest>
        /// </example> 
        public TransactionResult Post(TransactionRequest transaction)
        {
            return base.ProcessTransaction(transaction, User.Identity);
        }
    }
}
