using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Common.Utils;
using KeyHub.Core.Mail;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;
using KeyHub.Web.Api.Controllers.LicenseValidation;
using KeyHub.Web.Api.Controllers.Transaction;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;
using Microsoft.Ajax.Utilities;

namespace KeyHub.Web.Api.Controllers
{
    /// <summary>
    /// REST controller for creating new transactions.
    /// </summary>
    [Authorize]
    public class TransactionController : BaseTransactionController
    {
        public TransactionController(IDataContextFactory dataContextFactory, IMailService mailService)
            : base(dataContextFactory, mailService)
        {
        }

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
        ///     <TransactionRequest PurchaserName="Steven Somer" PurchaserEmail="steven@example.org">
        ///         <PurchasedSku>{guid}</PurchasedSku>
        ///     </TransactionRequest>
        /// </example> 
        public TransactionResult Post(TransactionRequest transaction)
        {
            IEnumerable<Guid> authorizedVendors;

            using (var dataContext = dataContextFactory.Create())
            {
                var identity = User.Identity;
                User currentUser = (from x in dataContext.Users where x.UserName == identity.Name select x).Include(x => x.Rights).FirstOrDefault();

                authorizedVendors = DataContextByUser.ResolveAuthorizedVendorsByUser(dataContext, currentUser);
            }

            return base.ProcessTransaction(transaction, authorizedVendors);
        }
    }
}
