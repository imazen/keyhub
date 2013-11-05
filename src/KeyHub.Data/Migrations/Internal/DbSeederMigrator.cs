using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using KeyHub.Common.Utils;

namespace KeyHub.Data.Migrations
{
    /// <summary>
    /// Provides advanced migrations by providing a seeding platform for each migration.
    /// This allows for initial seed data after each new database version (for example when 
    /// deploying new features and you want to include initial data). Seeders will be executing 
    /// in the correct order after all migrations have been completed.
    /// </summary>
    public class DbSeederMigrator<TContext> : DbMigrator where TContext : DbContext, new()
    {
        private static readonly System.Text.RegularExpressions.Regex _migrationIdPattern = new Regex(@"\d{15}_.+");
        private const string _migrationTypeFormat = "{0}.{1}, {2}";
        private const string _automaticMigration = "AutomaticMigration";

        /// <summary>
        /// Initializes a new instance of the DbMigrator class.
        /// </summary>
        /// <param name="configuration">Configuration to be used for the migration process.</param>
        public DbSeederMigrator(DbMigrationsConfiguration configuration)
            : base(configuration)
        { }

        /// <summary>
        /// Migrates the database to the latest version
        /// </summary>
        public void MigrateToLatestVersion()
        {
            var seedList = new List<IMigrationDataSeeder<TContext>>();

            // Apply migrations
            foreach (var migrationId in GetPendingMigrations())
            {
                if (IsAutomaticMigration(migrationId))
                    continue;

                if(!IsValidMigrationId(migrationId))
                    continue;

                var migration = GetMigrationFromMigrationId(migrationId);
                var migrationSeeder = GetSeederFromMigration(migration);

                // Call the actual update to execute this migration
                base.Update(migrationId);

                if (migrationSeeder != null)
                    seedList.Add(migrationSeeder);
            }

            // Create a new datacontext using the generic type provided
            TContext databaseContext = new TContext();

            // Apply data seeders
            foreach (var migrationSeeder in seedList)
            {
                migrationSeeder.Seed(databaseContext);
                databaseContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the IMigrationDataSeeder from the DbMigration if the interface was implemented
        /// </summary>
        /// <param name="migration">The instance of the migration to seed</param>
        /// <returns>The migration instance typed as IMigrationDataSeeder</returns>
        private IMigrationDataSeeder<TContext> GetSeederFromMigration(DbMigration migration)
        {
            return migration as IMigrationDataSeeder<TContext>;
        }

        /// <summary>
        /// Creates a full type for the migration class by using the current migrations namespace
        /// ie: Project.Core.Data.Migrations.34589734533_InitialCreate
        /// </summary>
        /// <param name="migrator">The migrator context</param>
        /// <param name="migrationId">The migration id from the migrations list of the migrator</param>
        /// <returns>The full DbMigration instance</returns>
        private DbMigration GetMigrationFromMigrationId(string migrationId)
        {
            string migrationTypeName = string.Format(_migrationTypeFormat,
                                                     Configuration.MigrationsNamespace,
                                                     GetMigrationClassName(migrationId),
                                                     Configuration.MigrationsAssembly.FullName);
            
            return Reflection.CreateTypeInstance<DbMigration>(migrationTypeName);
        }

        #region "Migration ID validation"

        /// <summary>
        /// Checks if the migration id is valid
        /// </summary>
        /// <param name="migrationId">The migration id from the migrations list of the migrator</param>
        /// <returns>true if valid, otherwise false</returns>
        /// <remarks>
        /// This snippet has been copied from the EntityFramework source (http://entityframework.codeplex.com/)
        /// </remarks>
        private bool IsValidMigrationId(string migrationId)
        {
            if (string.IsNullOrWhiteSpace(migrationId))
                return false;

            return _migrationIdPattern.IsMatch(migrationId)
                   || migrationId == DbMigrator.InitialDatabase;
        }

        /// <summary>
        /// Checks if the the migration id belongs to an automatic migration
        /// </summary>
        /// <param name="migrationId">The migration id from the migrations list of the migrator</param>
        /// <returns>true if automatic, otherwise false</returns>
        /// <remarks>
        /// This snippet has been copied from the EntityFramework source (http://entityframework.codeplex.com/)
        /// </remarks>
        private bool IsAutomaticMigration(string migrationId)
        {
            if (string.IsNullOrWhiteSpace(migrationId))
                return false;

            return migrationId.EndsWith(_automaticMigration, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the ClassName from a migration id
        /// </summary>
        /// <param name="migrationId">The migration id from the migrations list of the migrator</param>
        /// <returns>The class name for this migration id</returns>
        /// <remarks>
        /// This snippet has been copied from the EntityFramework source (http://entityframework.codeplex.com/)
        /// </remarks>
        private string GetMigrationClassName(string migrationId)
        {
            if (string.IsNullOrWhiteSpace(migrationId))
                return string.Empty;

            return migrationId.Substring(16);
        }

        #endregion

    }
}
