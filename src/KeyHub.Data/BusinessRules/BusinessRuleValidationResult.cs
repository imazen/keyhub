using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Data.BusinessRules
{
    public class BusinessRuleValidationResult
    {
        /// <summary>
        /// Represents the success of the validation
        /// </summary>
        public static readonly BusinessRuleValidationResult Success;

        /// <summary>
        /// Gets the name of the rule that throwed this result
        /// </summary>
        public string BusinessRuleName { get; set; }

        /// <summary>
        /// Gets the error message for the validation.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Constructs a new validation result class without a business role
        /// </summary>
        /// <param name="message">The error message to include</param>
        public BusinessRuleValidationResult(string message)
        {
            BusinessRuleName = "";
            ErrorMessage = message;
        }

        /// <summary>
        /// Constructs a new validation result class
        /// </summary>
        /// <param name="message">The error message to include</param>
        /// <pparam name="rule">The business rule that instantiated this result</pparam>
        public BusinessRuleValidationResult(string message, IBusinessRule rule)
            : this(message)
        {
            BusinessRuleName = rule.BusinessRuleName;
        }
    }
}
