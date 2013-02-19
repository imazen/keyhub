using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeyHub.Core.Mail;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;

namespace KeyHub.Web.Mail
{
    public class MailService : IMailService
    {
        public void SendTransactionMail(string purchaserName, string purchaserEmail, int transactionId)
        {
            var transactionEmail = new TransactionMailViewModel
            {
                PurchaserName = purchaserName,
                PurchaserEmail = purchaserEmail,
                TransactionId = transactionId
            };
            new MailController().TransactionEmail(transactionEmail).Deliver();
        }
    }
}