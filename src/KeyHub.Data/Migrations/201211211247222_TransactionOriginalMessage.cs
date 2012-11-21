namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionOriginalMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "OriginalRequest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "OriginalRequest");
        }
    }
}
