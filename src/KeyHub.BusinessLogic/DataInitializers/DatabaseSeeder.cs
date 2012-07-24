using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Runtime;

namespace KeyHub.BusinessLogic.DataInitializers
{
    /// <summary>
    /// Responsible for calling the correct seed classes to prefill the database
    /// </summary>
    internal class DatabaseSeeder
    {
        /// <summary>
        /// Prefills the database using seed classes
        /// </summary>
        /// <param name="context">instance of the datacontext</param>
        internal static void SeedDatabase(DataContext context)
        {
            // Initialize countries
            CountrySeed.Seed(context);

            // Initialize rights
            // TODO: create rights seed

            context.SaveChanges();
        }
    }
}