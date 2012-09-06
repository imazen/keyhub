using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ActionMailer.Net.Mvc;
using KeyHub.Web.ViewModels.Mail;

namespace KeyHub.Web.Controllers
{
    public class MailController : MailerBase
    {
        public EmailResult TransactionEmail(TransactionMailViewModel model)
        {
            To.Add(model.PurchaserEmail);
            From = "no-reply@lucrasoft.nl";
            Subject = "Please claim your transaction.";
            return Email("NewTransactionEmail", model);
        }
    }

}
