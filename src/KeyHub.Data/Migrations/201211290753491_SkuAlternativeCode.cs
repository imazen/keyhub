namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkuAlternativeCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SKUs", "SkuAternativeCode", c => c.String(nullable: false, maxLength: 256, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SKUs", "SkuAternativeCode");
        }
    }
}
