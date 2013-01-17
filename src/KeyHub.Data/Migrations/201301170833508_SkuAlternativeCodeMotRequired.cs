namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkuAlternativeCodeMotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SKUs", "SkuAternativeCode", c => c.String(maxLength: 256, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SKUs", "SkuAternativeCode", c => c.String(nullable: false, maxLength: 256, unicode: false));
        }
    }
}
