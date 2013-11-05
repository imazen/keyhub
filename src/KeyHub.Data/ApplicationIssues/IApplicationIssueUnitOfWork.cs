using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data.ApplicationIssues
{
    public interface IApplicationIssueUnitOfWork
    {
        /// <summary>
        /// The customer app this issue belongs to
        /// </summary>
        Guid CustomerAppId { get; set; }

        /// <summary>
        /// DateTime of occurance of the issue
        /// </summary>
        DateTime DateTime { get; set; }

        /// <summary>
        /// Severity of the issue
        /// </summary>
        ApplicationIssueSeverity Severity { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Details
        /// </summary>
        string Details { get; set; }

        /// <summary>
        /// Commit the ApplicationIssue to the model
        /// </summary>
        /// <returns></returns>
        CustomerAppIssue Commit();
    }
}
