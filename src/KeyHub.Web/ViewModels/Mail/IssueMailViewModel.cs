using KeyHub.Model;

namespace KeyHub.Web.ViewModels.Mail
{
    public class IssueMailViewModel
    {
        public string User { get; set; }
        public string Email { get; set; }
        public ApplicationIssueSeverity Severity { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}