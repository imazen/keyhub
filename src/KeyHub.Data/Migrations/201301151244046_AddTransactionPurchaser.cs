namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactionPurchaser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "PurchaserName", c => c.String(nullable: false, maxLength: 256));
            AddColumn("dbo.Transactions", "PurchaserEmail", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "PurchaserEmail");
            DropColumn("dbo.Transactions", "PurchaserName");
        }
    }
}
