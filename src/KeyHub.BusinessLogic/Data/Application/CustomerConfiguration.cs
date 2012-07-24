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
    public class CustomerConfiguration : EntityTypeConfiguration<Customer>, IEntityConfiguration
    {
        public CustomerConfiguration()
        {
            Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Customers");
            });
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}