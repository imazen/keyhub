using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Data.Migrations
{
    /// <summary>
    /// Provides data seeding capabities to the EF Migration classes
    /// </summary>
    public interface IMigrationDataSeeder<TContext> where TContext : DbContext
    {
        /// <summary>
        /// Seeds this migration
        /// </summary>
        void Seed(TContext context);
    }
}
