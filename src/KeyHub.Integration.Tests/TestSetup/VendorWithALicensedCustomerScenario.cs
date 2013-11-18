namespace KeyHub.Integration.Tests.TestSetup
{
    public class VendorWithALicensedCustomerScenario : WithAVendorScenario
    {
        public void Setup(KeyHubWebDriver site, bool canDeleteManualDomainsOfLicense = true)
        {
            using (var browser = BrowserUtil.GetBrowser())
            {
                base.Setup(site);

                browser.Navigate().GoToUrl(site.UrlFor("/"));
                SiteUtil.SubmitLoginForm(browser, UserEmail, UserPassword);

                VendorUtil.CreatePrivateKey(browser, VendorName);

                VendorUtil.CreateFeature(browser, "first feature", VendorName);
                VendorUtil.CreateFeature(browser, "second feature", VendorName);

                VendorUtil.CreateSku(browser, "first sku", VendorName, "first feature", canDeleteManualDomainsOfLicense);
                VendorUtil.CreateSku(browser, "second sku", VendorName, "second feature", canDeleteManualDomainsOfLicense);

                //  Create a Customer
                var customerName = VendorUtil.CreateCustomer(browser);

                //  Create a License
                VendorUtil.CreateLicense(browser, "first sku", customerName);
                VendorUtil.CreateLicense(browser, "second sku", customerName);
            }
        }
    }
}