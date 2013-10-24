namespace KeyHub.Client
{
    internal class DomainUtility
    {
        public static string NormalizeDomain(string domain)
        {
            //lowercase
            domain = domain.ToLowerInvariant();
            //Strip www prefix off.
            if (domain.StartsWith("www.")) domain = domain.Substring(4);

            return domain;
        }
    }
}