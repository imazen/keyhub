namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerAppIssue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerAppIssues",
                c => new
                    {
                        CustomerAppIssueId = c.Int(nullable: false, identity: true),
                        CustomerAppId = c.Guid(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        Severity = c.Int(nullable: false),
                        Message = c.String(),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.CustomerAppIssueId)
                .ForeignKey("dbo.CustomerApps", t => t.CustomerAppId, cascadeDelete: true)
                .Index(t => t.CustomerAppId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomerAppIssues", new[] { "CustomerAppId" });
            DropForeignKey("dbo.CustomerAppIssues", "CustomerAppId", "dbo.CustomerApps");
            DropTable("dbo.CustomerAppIssues");
        }
    }
}
