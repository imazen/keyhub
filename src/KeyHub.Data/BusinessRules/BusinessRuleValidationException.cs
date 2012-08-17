using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Data.BusinessRules
{
    /// <summary>
    /// Thrown when a business rule was executed and returned errors
    /// </summary>
    [Serializable]
    public class BusinessRuleValidationException : Exception
    {
        private const string exceptionMessage = "One or more business rules failed to be applied. Check the ValidationResults property for more information.";

        /// <summary>
        /// Gets a list of results from the business validators
        /// </summary>
        public IEnumerable<BusinessRuleValidationResult> ValidationResults { get; private set; }

        /// <summary>
        /// Creates a new <c>BusinessRuleValidationException</c> from a set of results
        /// </summary>
        /// <param name="results">A list of results from the business rules execution</param>
        public BusinessRuleValidationException(params BusinessRuleValidationResult[] results)
            : base(exceptionMessage)
        {
            ValidationResults = new List<BusinessRuleValidationResult>(results);
        }

        /// <summary>
        /// Gets a string representation of all the failed rules
        /// </summary>
        public override string ToString()
        {
            var validationSummary = new StringBuilder();

            validationSummary.AppendLine(this.Message);

            foreach (var validationResult in ValidationResults)
            {
                if (validationResult != BusinessRuleValidationResult.Success)
                {
                    validationSummary.AppendLine(validationResult.BusinessRuleName + ": " + validationResult.ErrorMessage);
                    validationSummary.AppendLine();
                }
            }

            return validationSummary.ToString();
        }
    }
}
