namespace KeyHub.Web.Api.Controllers.Transaction
{
    /// <summary>
    /// Transaction result object
    /// </summary>
    public class TransactionResult
    {
        public bool CreatedSuccessfull { get; set; }
        public string ErrorMessage { get; set; }
    }
}