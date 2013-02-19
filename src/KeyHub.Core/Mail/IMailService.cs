namespace KeyHub.Core.Mail
{
    public interface IMailService
    {
        void SendTransactionMail(string purchaserName, string purchaserEmail, int transactionId);
    }
}
