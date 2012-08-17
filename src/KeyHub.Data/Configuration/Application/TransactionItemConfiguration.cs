﻿using System;
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
    /// Configures the <see cref="KeyHub.Model.Transaction"/> table
    /// </summary>
    public class TransactionItemConfiguration : EntityTypeConfiguration<TransactionItem>, IEntityConfiguration
    {
        public TransactionItemConfiguration()
        {
            HasOptional(x => x.License).WithMany(x => x.TransactionItems).WillCascadeOnDelete(false);
            ToTable("TransactionItems");
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}