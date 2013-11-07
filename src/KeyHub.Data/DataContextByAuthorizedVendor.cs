using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KeyHub.Data
{
    public class DataContextByAuthorizedVendor : AbstractFilteredDataContext
    {
        private readonly Guid vendorId;

        public DataContextByAuthorizedVendor(Guid vendorId)
        {
            this.vendorId = vendorId;
            ApplyFilters();
        }

        protected override IEnumerable<Guid> ResolveAuthorizedVendors()
        {
            return new[] {vendorId};
        }

        protected override bool ContextIsForSystemAdmin()
        {
            return false;
        }

        protected override bool ContextIsForVendorAdmin()
        {
            return false;
        }

        protected override IEnumerable<Guid> GetUserCustomerRights()
        {
            return new Guid[0];
        }

        protected override IEnumerable<Guid> GetUserLicenseRights()
        {
            return new Guid[0];
        }
    }
}