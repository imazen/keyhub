using System;
using System.Web.Http;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Common.Utils;
using KeyHub.Data.BusinessRules;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;

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
        ///     <TransactionRequest PurchaserName="Steven Somer" PurchaserEmail="steven@lucrasoft.nl">
        ///         <PurchasedSku>{guid}</PurchasedSku>
        ///     </TransactionRequest>
        /// </example> 
        public TransactionResult Post(TransactionRequest transaction)
        {
            if (transaction == null)
                return new TransactionResult { CreatedSuccessfull = false, ErrorMessage = "Invalid transaction format provided" };
            if (string.IsNullOrEmpty(transaction.PurchaserName))
                return new TransactionResult { CreatedSuccessfull = false, ErrorMessage = "No purchaser name set" };
            if (string.IsNullOrEmpty(transaction.PurchaserEmail))
                return new TransactionResult { CreatedSuccessfull = false, ErrorMessage = "No purchaser email set" };
            if (!Strings.IsEmail(transaction.PurchaserEmail))
                return new TransactionResult { CreatedSuccessfull = false, ErrorMessage = string.Format("Purchaser email '{0}' is not an e-mailaddress", transaction.PurchaserEmail) };

            try
            {
                var basket = BasketWrapper.CreateNewByIdentity(User.Identity);

                basket.AddSkUs(transaction.PurchasedSkus);

                basket.ExecuteStep(BasketSteps.Create);

                var transactionEmail = new TransactionMailViewModel
                                           {
                                               PurchaserName = transaction.PurchaserName,
                                               PurchaserEmail = transaction.PurchaserEmail,
                                               TransactionId = basket.Transaction.TransactionId
                                           };
                new MailController().TransactionEmail(transactionEmail).Deliver();

                return new TransactionResult { CreatedSuccessfull = true };
            }
            catch(BusinessRuleValidationException e)
            {
// ReSharper disable RedundantToStringCall, expilicitly call diverted implementation of ToString()
                return new TransactionResult { CreatedSuccessfull = false, ErrorMessage = string.Format("Could not process transaction: {0}", e.ToString()) };
// ReSharper restore RedundantToStringCall
            }
            catch (Exception e)
            {
                return new TransactionResult { CreatedSuccessfull = false, ErrorMessage = string.Format("Could not process transaction due to exception: {0}", e.Message) };
            }
        }
    }
}
