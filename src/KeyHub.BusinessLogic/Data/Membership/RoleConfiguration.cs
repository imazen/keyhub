﻿using System;
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
    /// Configures the membership table
    /// </summary>
    public class RoleConfiguration : EntityTypeConfiguration<Role>, IEntityConfiguration
    {
        public RoleConfiguration()
        {
            HasMany(x => x.UsersInRoles).WithRequired(x => x.Role).WillCascadeOnDelete(false);
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}