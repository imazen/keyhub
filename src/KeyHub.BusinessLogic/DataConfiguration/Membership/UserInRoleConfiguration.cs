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
    /// Configures the <see cref="KeyHub.Model.UserInRole"/> table
    /// </summary>
    public class UserInRoleConfiguration : EntityTypeConfiguration<UserInRole>, IEntityConfiguration
    {
        public UserInRoleConfiguration()
        {
            ToTable("UsersInRoles");
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}