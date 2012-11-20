using System.Net.Http;

namespace KeyHub.Core.Transactions
{
    public interface ITransactionPostHandler
    {
        TransactionPostDetails InterpretTransactionPost(HttpRequestMessage data);
    }
}
