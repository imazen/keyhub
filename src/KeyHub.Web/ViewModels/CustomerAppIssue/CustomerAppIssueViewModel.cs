using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeyHub.Model;

namespace KeyHub.Web.ViewModels.CustomerAppIssue
{
    public class CustomerAppIssueViewModel : BaseViewModel<Model.CustomerAppIssue>
    {
        public CustomerAppIssueViewModel() : base() { }

        /// <summary>
        /// Construct viewmodel
        /// </summary>
        /// <param name="customerAppIssue">CustomerAppIssue that this viewmodel represents</param>
        public CustomerAppIssueViewModel(Model.CustomerAppIssue customerAppIssue)
            : this()
        {
            this.CustomerAppIssueId = customerAppIssue.CustomerAppIssueId;
            this.CustomerAppId = customerAppIssue.CustomerAppId;
            this.DateTime = customerAppIssue.DateTime;
            this.Severity = customerAppIssue.Severity;
            this.Message = customerAppIssue.Message;
            this.Details = customerAppIssue.Details;
        }

        /// <summary>
        /// Convert back to CustomerAppIssue instance
        /// </summary>
        /// <param name="original">Original CustomerAppIssue. If Null a new instance is created.</param>
        /// <returns>CustomerAppIssue containing viewmodel data </returns>
        public override Model.CustomerAppIssue ToEntity(Model.CustomerAppIssue original)
        {
            var current = original ?? new Model.CustomerAppIssue();

            current.CustomerAppIssueId = CustomerAppIssueId;
            current.CustomerAppId = CustomerAppId;
            current.DateTime = DateTime;
            current.Severity = Severity;
            current.Message = Message;
            current.Details = Details;

            return current;
        }

        /// <summary>
        /// Indentifier for the CustomerAppIssue entity.
        /// </summary>
        public int CustomerAppIssueId { get; set; }

        /// <summary>
        /// The customer app this issue belongs to
        /// </summary>
        public Guid CustomerAppId { get; set; }

        /// <summary>
        /// DateTime of occurance of the issue
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Severity of the issue
        /// </summary>
        public ApplicationIssueSeverity Severity { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Details
        /// </summary>
        public string Details { get; set; }
    }
}