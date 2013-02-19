using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

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
        /// Gets the entity propety that failed validation
        /// </summary>
        public string PropertyName { get; set; }

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
        /// <param name="rule">The business rule that instantiated this result</param>
        /// <param name="propertyName">The name of the property affected</param>
        public BusinessRuleValidationResult(string message, IBusinessRule rule, string propertyName)
            : this(message)
        {
            BusinessRuleName = rule.BusinessRuleName;
            PropertyName = propertyName;
        }
    }
}
