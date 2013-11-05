using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Data;
using KeyHub.Data.Migrations;
using Xunit;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class CleanDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            Database.Delete("DataContext");

            var configuration = new MigrationConfiguration();
            var migrator = new DbSeederMigrator<DataContext>(configuration);
            migrator.MigrateToLatestVersion();
        }
    }
}
