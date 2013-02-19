using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Mail;
using KeyHub.Model;

namespace KeyHub.Tests.TestCore
{
    public class FakeMailService : IMailService
    {
        public virtual void SendTransactionMail(string purchaserName, string purchaserEmail, int transactionId)
        {
            //What should the mail service under test do?
        }

        public void SendIssueMail(ApplicationIssueSeverity severity, string message, string details, IEnumerable<User> users)
        {
            
        }
    }
}
