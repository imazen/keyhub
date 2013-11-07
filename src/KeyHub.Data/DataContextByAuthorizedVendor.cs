using System;
using System.Collections.Generic;
using KeyHub.Model;

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

        protected override ICollection<UserObjectRight> GetNonvendorUserObjectRights()
        {
            return new UserObjectRight[0];
        }
    }
}