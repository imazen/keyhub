namespace KeyHub.Common
{
    public static class Constants
    {
        public static readonly string ConnectionStringName = "DataContext";
        public static readonly string DefaultPasswordEncryption = "KeyHubAbcd!@#";
        public static readonly string BasketCookieName = "KeyHub_BaseketCookie";
        public static readonly int BasketCookieExpirationDays = 1;

        public static readonly string LicensesTag = "licenses";
        public static readonly string LicenseTag = "license";
        public static readonly string LicenseSignatureTag = "signature";
        public static readonly string LicenseValueTag = "value";

        //Send out a warning 4 weeks in advance
        public const int LicenseExpireWarningDays = 4 * 7;
    }
}
