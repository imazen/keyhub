namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class MigrationConfiguration : DbMigrationsConfiguration<KeyHub.Data.DataContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
