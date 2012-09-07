using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using KeyHub.Web.ViewModels.Mail;

namespace KeyHub.Web.Controllers
{
    /// <summary>
    /// Controller for sending mails
    /// </summary>
    /// <remarks>
    /// Inherits from ActionMailer's MailerBase
    /// </remarks>
    [Authorize]
    public class MailController : MailerBase
    {
        /// <summary>
        /// Send out a TransactionEmail
        /// </summary>
        /// <param name="model">TransactionMailViewModel containing transaction details</param>
        /// <returns>Emailmessage ready to be set to purchaser</returns>
        public EmailResult TransactionEmail(TransactionMailViewModel model)
        {
            To.Add(model.PurchaserEmail);
            From = "no-reply@lucrasoft.nl";
            Subject = "Please claim your transaction.";
            return Email("NewTransactionEmail", model);
        }
    }

}
