﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;
using KeyHub.BusinessLogic.Basket;
using KeyHub.Common.Utils;
using KeyHub.Core.Mail;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Web.Api.Controllers.Transaction;
using KeyHub.Web.Controllers;
using KeyHub.Web.ViewModels.Mail;

namespace KeyHub.Web.Api.Controllers.LicenseValidation
{
    /// <summary>
    /// Base REST controller for creating new transactions.
    /// </summary>
    public abstract class BaseTransactionController : ApiController
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly IMailService mailService;
        public BaseTransactionController(IDataContextFactory dataContextFactory, IMailService mailService)
        {
            this.dataContextFactory = dataContextFactory;
            this.mailService = mailService;
        }

        protected TransactionResult ProcessTransaction(TransactionRequest transaction, IIdentity userIdentity)
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
                var basket = BasketWrapper.CreateNewByIdentity(dataContextFactory);

                basket.AddItems(transaction.PurchasedSkus);

                basket.Transaction.OriginalRequest = GetOriginalRequestValues();
                basket.Transaction.PurchaserName = transaction.PurchaserName;
                basket.Transaction.PurchaserEmail = transaction.PurchaserEmail;

                basket.ExecuteStep(BasketSteps.Create);

                mailService.SendTransactionMail(transaction.PurchaserName,
                                                transaction.PurchaserEmail,
                                                basket.Transaction.TransactionId);

                return new TransactionResult { CreatedSuccessfull = true };
            }
            catch (BusinessRuleValidationException e)
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

        /// <summary>
        /// Reads the original request message and returns it a string
        /// </summary>
        /// <returns>Original request message</returns>
        /// <remarks>
        /// Unfortunately the WebApi requestbody is a forward only stream. Once the ModelBinder has
        /// parced the request to an object, the Request.Content is empty.
        /// For now HttpContext is used. On a non IIS deployment another solution has to be found.
        /// </remarks>
        private static string GetOriginalRequestValues()
        {
            if (HttpContext.Current == null)
                throw new NotSupportedException("Request body cannnot be read on a non IIS environment");

            var stream = HttpContext.Current.Request.InputStream;
            stream.Position = 0;

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets attribute value from original request message. 
        /// Attributes names: PurchaserName, PurchaserEmail.
        /// </summary>
        /// <returns>attribute value from original request message</returns>
        private static string GetOriginalRequestAttributeValue(string originalRequest, string attributeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(originalRequest);

            XmlElement root = doc.DocumentElement;

            return root != null ? root.Attributes[attributeName].Value : string.Empty;
        }
    }
}
