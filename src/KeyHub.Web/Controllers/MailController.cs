using ActionMailer.Net.Mvc;
using KeyHub.Web.ViewModels.Mail;
using System.Web.Configuration;
using System.Web.Mvc;

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
            bool redirectMails = bool.Parse(WebConfigurationManager.AppSettings["redirectMails"]);
            string redirectTo = WebConfigurationManager.AppSettings["redirectTo"];

            To.Add(redirectMails ? redirectTo : model.PurchaserEmail);
            From = "no-reply@lucrasoft.nl";
            Subject = "Please claim your transaction.";
            return Email("NewTransactionEmail", model);
        }
    }

}
