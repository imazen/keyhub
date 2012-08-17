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
    /// Configures the <see cref="KeyHub.Model.Feature"/> table
    /// </summary>
    public class FeatureConfiguration : EntityTypeConfiguration<Feature>, IEntityConfiguration
    {
        public FeatureConfiguration()
        {
            HasMany(x => x.SkuFeatures).WithRequired(x => x.Feature).WillCascadeOnDelete(true);
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}