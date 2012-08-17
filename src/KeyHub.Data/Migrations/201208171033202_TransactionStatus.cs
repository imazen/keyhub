namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "Status");
        }
    }
}
