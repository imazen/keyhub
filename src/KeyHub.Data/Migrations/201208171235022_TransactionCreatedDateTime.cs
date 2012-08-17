namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionCreatedDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "CreatedDateTime", c => c.DateTime(nullable: false, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "CreatedDateTime");
        }
    }
}
