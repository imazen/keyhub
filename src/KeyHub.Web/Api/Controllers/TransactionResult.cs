using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// Transaction result object
    /// </summary>
    public class TransactionResult
    {
        public bool CreatedSuccessfull { get; set; }
        public string ErrorMessage { get; set; }
    }
}