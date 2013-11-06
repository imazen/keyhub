using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Data;
using KeyHub.Model;

namespace KeyHub.Data.DataConfiguration
{
    /// <summary>
    /// Configures the <see cref="KeyHub.Model.Vendor"/> table
    /// </summary>
    public class VendorConfiguration : EntityTypeConfiguration<Vendor>, IEntityConfiguration
    {
        public VendorConfiguration()
        {
            Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Vendors");
            });

            HasMany(x => x.VendorCredentials).WithRequired(x => x.Vendor);
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}