using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using KeyHub.Data;
using KeyHub.Data.BusinessRules;
using KeyHub.Model;

namespace KeyHub.BusinessLogic.BusinessRules
{
    public class SkuExpirationRule : BusinessRule<TransactionItem>
    {
        private readonly IDataContextFactory dataContextFactory;

        public SkuExpirationRule(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Validates an TransactionItem to see if the SKU is still valid
        /// </summary>
        /// <returns>A collection of errors, or an empty collection if the business rule succeeded</returns>
        protected override IEnumerable<BusinessRuleValidationResult> ExecuteValidation(TransactionItem entity, DbEntityEntry entityEntry)
        {
            //Uses a full access datacontext, during license claim the user has no sufficient 
            //rights to see details of the SKU untill it is acutually added to the transactionItem

            using (var fullContext = dataContextFactory.Create())
            {
                var sku =
                    (from x in fullContext.SKUs
                     where x.SkuId == entity.SkuId
                     select x).FirstOrDefault();

                if (sku != null)
                {
                    if ((!sku.ExpirationDate.HasValue) || (sku.ExpirationDate.Value >= DateTime.Today))
                    {
                        yield return BusinessRuleValidationResult.Success;
                    }
                    else
                    {
                        yield return
                            new BusinessRuleValidationResult(
                                string.Format("TransactionItem SKU '{0}' has expired on {1}.",
                                              sku.SkuCode, sku.ExpirationDate.Value.ToShortDateString()), this, "SkuId")
                            ;
                    }
                }
                else
                {
                    yield return
                        new BusinessRuleValidationResult(
                            string.Format("TransactionItem SKU with ID '{0}' does not exist.",
                                          entity.SkuId), this, "SkuId");
                }
            }
        }

        /// <summary>
        /// Gets the business rule name
        /// </summary>
        public override string BusinessRuleName
        {
            get { return "SKU expired validation"; }
        }
    }
}
