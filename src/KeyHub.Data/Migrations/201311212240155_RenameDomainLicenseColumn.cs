namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDomainLicenseColumn : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DomainLicenses", "AutomaticlyCreated", "AutomaticallyCreated");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DomainLicenses", "AutomaticallyCreated", "AutomaticlyCreated");
        }
    }
}
