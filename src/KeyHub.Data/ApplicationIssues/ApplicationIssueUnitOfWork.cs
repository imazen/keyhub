using System;
using System.Linq;
using KeyHub.Core.Mail;
using KeyHub.Core.UnitOfWork;
using KeyHub.Model;

namespace KeyHub.Data.ApplicationIssues
{
    public class ApplicationIssueUnitOfWork : IUnitOfWork<CustomerAppIssue>, IApplicationIssueUnitOfWork
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly IMailService mailService;
        public ApplicationIssueUnitOfWork(IDataContextFactory dataContextFactory, IMailService mailService)
        {
            this.dataContextFactory = dataContextFactory;
            this.mailService = mailService;
        }

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

        /// <summary>
        /// Commit the ApplicationIssue to the model
        /// </summary>
        /// <returns></returns>
        public CustomerAppIssue Commit()
        {
            using (var context = dataContextFactory.Create())
            {
                var customerAppIssue = new CustomerAppIssue
                    {
                        CustomerAppId = this.CustomerAppId,
                        DateTime = this.DateTime,
                        Severity = this.Severity,
                        Message = this.Message,
                        Details = this.Details
                    };

                context.CustomerAppIssues.Add(customerAppIssue);
                context.SaveChanges();

                //Resolve users to mail to
                var licenseIds = (from ca in context.CustomerApps
                                  join cla in context.LicenseCustomerApps on ca.CustomerAppId equals cla.CustomerAppId
                                  join l in context.Licenses on cla.LicenseId equals l.ObjectId
                                  where ca.CustomerAppId == CustomerAppId 
                                  select l.ObjectId).ToList();

                var licenseUsers = (from u in context.Users
                                    join r in context.UserLicenseRights on u.UserId equals r.UserId
                                    where licenseIds.Contains(r.ObjectId)
                                    select u).ToList();

                var customerUsers = (from u in context.Users
                                    join r in context.UserCustomerRights on u.UserId equals r.UserId
                                    join c in context.Customers on r.ObjectId equals c.ObjectId
                                    join l in context.Licenses on c.ObjectId equals  l.OwningCustomerId
                                    where licenseIds.Contains(l.ObjectId)
                                     select u).ToList();

                var authorizedUsers = licenseUsers.Union(customerUsers).Distinct();

                mailService.SendIssueMail(Severity, Message, Details, authorizedUsers);

                return customerAppIssue;
            }
        }

        /// <summary>
        /// Dispose the ApplicationIssue unit of work
        /// </summary>
        public void Dispose()
        {
        }
    }
}
