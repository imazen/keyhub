using KeyHub.Model;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class WithAVendorScenario
    {
        //  The vendor is the user, VendorName is the name of the Vendor object not the User.
        public string UserEmail = "vendor@example.com";
        public string UserPassword = "vendorPassword";
        public string VendorName;

        public void Setup(KeyHubWebDriver site)
        {
            //  The vendor creates their user account
            SiteUtil.CreateLocalAccount(site, UserEmail, UserPassword);

            //  The admin makes that user account a vendor.
            using (var browser = BrowserUtil.GetBrowser())
            {
                browser.Navigate().GoToUrl(site.UrlFor("/"));
                SiteUtil.SubmitLoginForm(browser, "admin", "password");

                VendorName = AdminUtil.CreateVendor(browser);

                AdminUtil.CreateAccountRightsFor(browser, UserEmail, ObjectTypes.Vendor,
                    VendorName);
            }
        }
    }
}