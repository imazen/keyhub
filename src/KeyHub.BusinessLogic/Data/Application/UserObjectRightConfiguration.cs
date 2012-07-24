using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Data;
using KeyHub.Model;

namespace KeyHub.BusinessLogic.Data
{
    /// <summary>
    /// Configures the SkuFeatures table
    /// </summary>
    public class UserObjectRightConfiguration : EntityTypeConfiguration<UserObjectRight>, IEntityConfiguration
    {
        public UserObjectRightConfiguration()
        {
            Map<UserVendorRight>(m => m.Requires("ObjectType").HasValue((int)ObjectTypes.Vendor));
            Map<UserCustomerRight>(m => m.Requires("ObjectType").HasValue((int)ObjectTypes.Customer));
            Map<UserLicenseRight>(m => m.Requires("ObjectType").HasValue((int)ObjectTypes.License));
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}