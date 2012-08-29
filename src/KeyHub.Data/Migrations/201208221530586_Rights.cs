namespace KeyHub.Data.Migrations
{
    using KeyHub.Model;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Rights : DbMigration, IMigrationDataSeeder<DataContext>
    {
        public override void Up()
        {
            AlterColumn("dbo.Rights", "RightId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Rights", "DisplayName", c => c.String(nullable: false, maxLength: 256, unicode: false));
            DropColumn("dbo.Customers", "PayPalId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "PayPalId", c => c.String(maxLength: 256));
            AlterColumn("dbo.Rights", "DisplayName", c => c.String(maxLength: 256, unicode: false));
            AlterColumn("dbo.Rights", "RightId", c => c.Guid(nullable: false, identity: true));
        }

        public void Seed(DataContext context)
        {
            // Get all countries from the framework and insert them into the table
            foreach (var right in KeyHub.Runtime.DependencyContext.Instance.GetExportedValues<IRight>())
            {
                context.Rights.Add(new Right() { DisplayName = right.DisplayName, RightId= right.RightId });
            }
        }
    }
}
