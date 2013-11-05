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
    /// Configures the <see cref="KeyHub.Model.OAuthMembership"/> table
    /// </summary>
    public class OAuthMembershipConfiguration : EntityTypeConfiguration<OAuthMembership>, IEntityConfiguration
    {
        /// <summary>
        /// Configures the OAuthMembership table to the correct tablename.
        /// Note: Name is forced by WebSecurity
        /// </summary>
        public OAuthMembershipConfiguration()
        {
            ToTable("webpages_OAuthMembership");
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}