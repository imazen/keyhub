using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyHub.Web.ViewModels.Mail
{
    public class TransactionMailViewModel
    {
        public string PurchaserName { get; set; }
        public string PurchaserEmail { get; set; }
        public int TransactionId { get; set; }
    }
}
