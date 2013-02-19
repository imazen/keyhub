using System.Configuration;
using ActionMailer.Net.Mvc;
using KeyHub.Data;
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
    public class MailController : MailerBase
    {
        /// <summary>
        /// Send out a TransactionEmail
        /// </summary>
        /// <param name="model">TransactionMailViewModel containing transaction details</param>
        /// <returns>Emailmessage ready to be set to purchaser</returns>
        public EmailResult TransactionEmail(TransactionMailViewModel model)
        {
            bool redirectMails = (WebConfigurationManager.AppSettings["redirectMails"]!=null) ? bool.Parse(WebConfigurationManager.AppSettings["redirectMails"]) : false;
            string redirectTo = WebConfigurationManager.AppSettings["redirectTo"];

            if (redirectMails && string.IsNullOrEmpty(redirectTo))
                throw new ConfigurationErrorsException("Mail redirecting enabled without a RedirectTo set");

            To.Add(redirectMails ? redirectTo : model.PurchaserEmail);
            From = "no-reply@lucrasoft.nl";
            Subject = "Please claim your transaction.";
            return Email("NewTransactionEmail", model);
        }

        /// <summary>
        /// Send out a IssueEmail
        /// </summary>
        /// <param name="model">IssueMailViewModel containing issue details</param>
        /// <returns>Emailmessage ready to be set to purchaser</returns>
        public EmailResult IssueEmail(IssueMailViewModel model)
        {
            bool redirectMails = (WebConfigurationManager.AppSettings["redirectMails"] != null) ? bool.Parse(WebConfigurationManager.AppSettings["redirectMails"]) : false;
            string redirectTo = WebConfigurationManager.AppSettings["redirectTo"];

            if (redirectMails && string.IsNullOrEmpty(redirectTo))
                throw new ConfigurationErrorsException("Mail redirecting enabled without a RedirectTo set");

            To.Add(redirectMails ? redirectTo : model.Email);
            From = "no-reply@lucrasoft.nl";
            Subject = "An issue occured on you application.";
            return Email("IssueEmail", model);
        }
    }

}
