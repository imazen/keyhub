using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Core.Data;
using KeyHub.Model;

namespace KeyHub.BusinessLogic.DataConfiguration
{
    /// <summary>
    /// Configures the <see cref="KeyHub.Model.User"/> table
    /// </summary>
    public class UserConfiguration : EntityTypeConfiguration<User>, IEntityConfiguration
    {
        public UserConfiguration()
        {
            HasOptional<Profile>(p => p.Profile).WithRequired(x => x.User);
            HasMany(x => x.UsersInRoles).WithRequired(x => x.User).WillCascadeOnDelete(true);
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}