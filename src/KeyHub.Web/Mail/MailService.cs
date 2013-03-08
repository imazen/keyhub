using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeyHub.Core.Mail;
using KeyHub.Model;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;

namespace KeyHub.Web.Mail
{
    public class MailService : IMailService
    {
        public void SendTransactionMail(string purchaserName, string purchaserEmail, Guid transactionId)
        {
            var transactionEmail = new TransactionMailViewModel
            {
                PurchaserName = purchaserName,
                PurchaserEmail = purchaserEmail,
                TransactionId = transactionId
            };
            new MailController().TransactionEmail(transactionEmail).Deliver();
        }

        public void SendIssueMail(ApplicationIssueSeverity severity, string message, string details, IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                var issueEmail = new IssueMailViewModel
                {
                    User = user.UserName,
                    Email = user.Email,
                    Severity = severity,
                    Message = message,
                    Details = details
                };
                new MailController().IssueEmail(issueEmail).Deliver();
            }
        }
    }
}