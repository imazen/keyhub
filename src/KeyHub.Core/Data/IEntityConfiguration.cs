using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Core.Data
{
    /// <summary>
    /// Represents a configuration class for the EF ModelBuilder.
    /// </summary>
    [InheritedExport(typeof(IEntityConfiguration))]
    public interface IEntityConfiguration
    {
        /// <summary>
        /// Add configuration class to Configuration Container <see cref="ConfigurationRegistrar"/>
        /// </summary>
        /// <param name="registrar">the <see cref="ConfigurationRegistrar"/> to add configurations to</param>
        void AddConfiguration(ConfigurationRegistrar registrar);
    }
}