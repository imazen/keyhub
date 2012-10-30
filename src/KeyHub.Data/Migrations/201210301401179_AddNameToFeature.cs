namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToFeature : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Features", "FeatureName", c => c.String(nullable: false, maxLength: 256, unicode: false));
            AlterColumn("dbo.Features", "FeatureCode", c => c.Guid(nullable: false));
            AlterColumn("dbo.Customers", "Department", c => c.String(maxLength: 512));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "Department", c => c.String(nullable: false, maxLength: 512));
            AlterColumn("dbo.Features", "FeatureCode", c => c.String(nullable: false, maxLength: 256, unicode: false));
            DropColumn("dbo.Features", "FeatureName");
        }
    }
}
