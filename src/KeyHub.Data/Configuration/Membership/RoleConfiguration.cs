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
    /// Configures the <see cref="KeyHub.Model.Role"/> table
    /// </summary>
    public class RoleConfiguration : EntityTypeConfiguration<Role>, IEntityConfiguration
    {
        /// <summary>
        /// Configures the Roles table to the correct tablename.
        /// Note: Name is forced by WebSecurity
        /// </summary>
        public RoleConfiguration()
        {
            ToTable("webpages_Roles");

            HasMany(x => x.UsersInRoles).WithRequired(x => x.Role).WillCascadeOnDelete(false);
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}