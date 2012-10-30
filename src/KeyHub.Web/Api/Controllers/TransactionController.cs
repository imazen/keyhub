using System;
using System.Linq;
using System.Web.Http;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Common.Utils;
using KeyHub.Data;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;
using KeyHub.Web.ViewModels.Transaction;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// REST controller for creating new transactions.
    /// </summary>
    public class TransactionController : ApiController
    {
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
        ///     <?xml version="1.0" encoding="utf-8"?>
        ///     <TransactionRequest PurchaserName="Steven Somer" PurchaserEmail="steven@lucrasoft.nl">
        ///         <PurchasedSku>{guid}</PurchasedSku>
        ///     </TransactionRequest>
        /// </example> 
        public TransactionResult Post(TransactionRequest transaction)
        {
            if (transaction == null)
                throw new InvalidPropertyException("No transaction provided");
            if (string.IsNullOrEmpty(transaction.PurchaserName))
                throw new InvalidPropertyException("No purchaser name set");
            if (string.IsNullOrEmpty(transaction.PurchaserEmail))
                throw new InvalidPropertyException("No purchaser email set");
            if (!Strings.IsEmail(transaction.PurchaserEmail))
                throw new InvalidPropertyException(string.Format("Purchaser email '{0}' is not an e-mailaddress", transaction.PurchaserEmail));

            BasketWrapper basket = BasketWrapper.CreateNewByIdentity(User.Identity);

            basket.AddSKUs(transaction.PurchasedSkus);

            basket.ExecuteStep(BasketSteps.Create);

            var transactionEmail = new TransactionMailViewModel() { PurchaserName = transaction.PurchaserName, PurchaserEmail = transaction.PurchaserEmail, TransactionId = basket.Transaction.TransactionId };
            new MailController().TransactionEmail(transactionEmail).Deliver();

            var result = new TransactionResult() { CreatedSuccessfull = true };
            return result;
        }
    }
}
