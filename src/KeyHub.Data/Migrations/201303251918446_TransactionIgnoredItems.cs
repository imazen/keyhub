namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionIgnoredItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionIgnoredItems",
                c => new
                    {
                        TransactionItemId = c.Guid(nullable: false, identity: true),
                        TransactionId = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 1024),
                    })
                .PrimaryKey(t => t.TransactionItemId)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .Index(t => t.TransactionId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TransactionIgnoredItems", new[] { "TransactionId" });
            DropForeignKey("dbo.TransactionIgnoredItems", "TransactionId", "dbo.Transactions");
            DropTable("dbo.TransactionIgnoredItems");
        }
    }
}
