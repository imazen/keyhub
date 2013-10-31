namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.webpages_Membership",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        CreateDate = c.DateTime(),
                        ConfirmationToken = c.String(maxLength: 128),
                        IsConfirmed = c.Boolean(nullable: false),
                        LastPasswordFailureDate = c.DateTime(),
                        PasswordFailuresSinceLastSuccess = c.Int(nullable: false),
                        Password = c.String(nullable: false, maxLength: 128),
                        PasswordChangedDate = c.DateTime(),
                        PasswordSalt = c.String(nullable: false, maxLength: 128),
                        PasswordVerificationToken = c.String(maxLength: 128),
                        PasswordVerificationTokenExirationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Email = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Email, unique:true);
            
            CreateTable(
                "dbo.webpages_UsersInRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.webpages_Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.webpages_Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.SKUs",
                c => new
                    {
                        SkuId = c.Guid(nullable: false, identity: true),
                        VendorId = c.Guid(nullable: false),
                        PrivateKeyId = c.Guid(nullable: false),
                        SkuCode = c.String(nullable: false, maxLength: 256, unicode: false),
                        SkuAternativeCode = c.String(maxLength: 256, unicode: false),
                        MaxDomains = c.Int(),
                        EditOwnershipDuration = c.Int(),
                        MaxSupportContacts = c.Int(),
                        EditSupportContactsDuration = c.Int(),
                        LicenseDuration = c.Int(),
                        AutoDomainDuration = c.Int(),
                        ManualDomainDuration = c.Int(),
                        CanDeleteAutoDomains = c.Boolean(nullable: false),
                        CanDeleteManualDomains = c.Boolean(nullable: false),
                        ReleaseDate = c.DateTime(storeType: "datetime2"),
                        ExpirationDate = c.DateTime(storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.SkuId)
                .ForeignKey("dbo.PrivateKeys", t => t.PrivateKeyId)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.PrivateKeyId)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryCode = c.String(nullable: false, maxLength: 12, unicode: false),
                        CountryName = c.String(nullable: false, maxLength: 512, unicode: false),
                        NativeCountryName = c.String(nullable: false, maxLength: 512),
                    })
                .PrimaryKey(t => t.CountryCode);
            
            CreateTable(
                "dbo.PrivateKeys",
                c => new
                    {
                        PrivateKeyId = c.Guid(nullable: false, identity: true),
                        VendorId = c.Guid(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 256, unicode: false),
                        KeyBytes = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.PrivateKeyId)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.SkuFeatures",
                c => new
                    {
                        SkuId = c.Guid(nullable: false),
                        FeatureId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SkuId, t.FeatureId })
                .ForeignKey("dbo.Features", t => t.FeatureId, cascadeDelete: true)
                .ForeignKey("dbo.SKUs", t => t.SkuId)
                .Index(t => t.FeatureId)
                .Index(t => t.SkuId);
            
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        FeatureId = c.Guid(nullable: false, identity: true),
                        VendorId = c.Guid(nullable: false),
                        FeatureCode = c.Guid(nullable: false),
                        FeatureName = c.String(nullable: false, maxLength: 256, unicode: false),
                    })
                .PrimaryKey(t => t.FeatureId)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.DomainLicenses",
                c => new
                    {
                        DomainLicenseId = c.Guid(nullable: false, identity: true),
                        LicenseId = c.Guid(nullable: false),
                        DomainName = c.String(nullable: false, maxLength: 256, unicode: false),
                        DomainLicenseIssued = c.DateTime(nullable: false, storeType: "datetime2"),
                        DomainLicenseExpires = c.DateTime(storeType: "datetime2"),
                        AutomaticlyCreated = c.Boolean(nullable: false),
                        KeyBytes = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.DomainLicenseId)
                .ForeignKey("dbo.Licenses", t => t.LicenseId, cascadeDelete: true)
                .Index(t => t.LicenseId);
            
            CreateTable(
                "dbo.TransactionItems",
                c => new
                    {
                        TransactionItemId = c.Guid(nullable: false, identity: true),
                        TransactionId = c.Guid(nullable: false),
                        LicenseId = c.Guid(),
                        SkuId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionItemId)
                .ForeignKey("dbo.Transactions", t => t.TransactionId)
                .ForeignKey("dbo.Licenses", t => t.LicenseId)
                .ForeignKey("dbo.SKUs", t => t.SkuId, cascadeDelete: true)
                .Index(t => t.TransactionId)
                .Index(t => t.LicenseId)
                .Index(t => t.SkuId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Guid(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(nullable: false, storeType: "datetime2"),
                        OriginalRequest = c.String(),
                        PurchaserName = c.String(nullable: false, maxLength: 256),
                        PurchaserEmail = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.TransactionId);
            
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
            
            CreateTable(
                "dbo.LicenseCustomerApps",
                c => new
                    {
                        LicenseId = c.Guid(nullable: false),
                        CustomerAppId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LicenseId, t.CustomerAppId })
                .ForeignKey("dbo.Licenses", t => t.LicenseId, cascadeDelete: true)
                .ForeignKey("dbo.CustomerApps", t => t.CustomerAppId, cascadeDelete: true)
                .Index(t => t.LicenseId)
                .Index(t => t.CustomerAppId);
            
            CreateTable(
                "dbo.CustomerApps",
                c => new
                    {
                        CustomerAppId = c.Guid(nullable: false, identity: true),
                        ApplicationName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.CustomerAppId);
            
            CreateTable(
                "dbo.CustomerAppKeys",
                c => new
                    {
                        AppKey = c.Guid(nullable: false, identity: true),
                        CustomerAppId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.AppKey)
                .ForeignKey("dbo.CustomerApps", t => t.CustomerAppId)
                .Index(t => t.CustomerAppId);
            
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
            
            CreateTable(
                "dbo.Rights",
                c => new
                    {
                        RightId = c.Guid(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 256, unicode: false),
                    })
                .PrimaryKey(t => t.RightId);
            
            CreateTable(
                "dbo.UserObjectRights",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ObjectId = c.Guid(nullable: false),
                        RightId = c.Guid(nullable: false),
                        ObjectType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ObjectId, t.RightId })
                .ForeignKey("dbo.Rights", t => t.RightId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RightId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.webpages_OAuthMembership",
                c => new
                    {
                        Provider = c.String(nullable: false, maxLength: 30),
                        ProviderUserId = c.String(nullable: false, maxLength: 100),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Provider, t.ProviderUserId });
            
            CreateTable(
                "dbo.Vendors",
                c => new
                    {
                        ObjectId = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 512),
                        Street = c.String(nullable: false, maxLength: 512),
                        PostalCode = c.String(nullable: false, maxLength: 24),
                        City = c.String(nullable: false, maxLength: 256),
                        Region = c.String(nullable: false, maxLength: 256),
                        CountryCode = c.String(nullable: false, maxLength: 12, unicode: false),
                    })
                .PrimaryKey(t => t.ObjectId)
                .ForeignKey("dbo.Countries", t => t.CountryCode, cascadeDelete: true)
                .Index(t => t.CountryCode);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        ObjectId = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        Department = c.String(maxLength: 512),
                        Street = c.String(nullable: false, maxLength: 512),
                        PostalCode = c.String(nullable: false, maxLength: 24),
                        City = c.String(nullable: false, maxLength: 256),
                        Region = c.String(nullable: false, maxLength: 256),
                        CountryCode = c.String(nullable: false, maxLength: 12, unicode: false),
                    })
                .PrimaryKey(t => t.ObjectId)
                .ForeignKey("dbo.Countries", t => t.CountryCode, cascadeDelete: true)
                .Index(t => t.CountryCode);
            
            CreateTable(
                "dbo.Licenses",
                c => new
                    {
                        ObjectId = c.Guid(nullable: false, identity: true),
                        SkuId = c.Guid(nullable: false),
                        PurchasingCustomerId = c.Guid(nullable: false),
                        OwnerName = c.String(nullable: false, maxLength: 1024),
                        OwningCustomerId = c.Guid(nullable: false),
                        LicenseIssued = c.DateTime(nullable: false, storeType: "datetime2"),
                        LicenseExpires = c.DateTime(storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ObjectId)
                .ForeignKey("dbo.SKUs", t => t.SkuId, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.PurchasingCustomerId)
                .ForeignKey("dbo.Customers", t => t.OwningCustomerId)
                .Index(t => t.SkuId)
                .Index(t => t.PurchasingCustomerId)
                .Index(t => t.OwningCustomerId);

            CreateTable(
                "dbo.VendorSecrets",
                c => new
                {
                    VendorSecretId = c.Guid(nullable: false, identity: true),
                    VendorId = c.Guid(nullable: false),
                    Name = c.String(nullable: false),
                    SharedSecret = c.String(nullable: false)
                })
                .PrimaryKey(t => t.VendorSecretId)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Licenses", new[] { "OwningCustomerId" });
            DropIndex("dbo.Licenses", new[] { "PurchasingCustomerId" });
            DropIndex("dbo.Licenses", new[] { "SkuId" });
            DropIndex("dbo.Customers", new[] { "CountryCode" });
            DropIndex("dbo.Vendors", new[] { "CountryCode" });
            DropIndex("dbo.UserObjectRights", new[] { "UserId" });
            DropIndex("dbo.UserObjectRights", new[] { "RightId" });
            DropIndex("dbo.CustomerAppIssues", new[] { "CustomerAppId" });
            DropIndex("dbo.CustomerAppKeys", new[] { "CustomerAppId" });
            DropIndex("dbo.LicenseCustomerApps", new[] { "CustomerAppId" });
            DropIndex("dbo.LicenseCustomerApps", new[] { "LicenseId" });
            DropIndex("dbo.TransactionIgnoredItems", new[] { "TransactionId" });
            DropIndex("dbo.TransactionItems", new[] { "SkuId" });
            DropIndex("dbo.TransactionItems", new[] { "LicenseId" });
            DropIndex("dbo.TransactionItems", new[] { "TransactionId" });
            DropIndex("dbo.DomainLicenses", new[] { "LicenseId" });
            DropIndex("dbo.Features", new[] { "VendorId" });
            DropIndex("dbo.SkuFeatures", new[] { "SkuId" });
            DropIndex("dbo.SkuFeatures", new[] { "FeatureId" });
            DropIndex("dbo.PrivateKeys", new[] { "VendorId" });
            DropIndex("dbo.SKUs", new[] { "VendorId" });
            DropIndex("dbo.SKUs", new[] { "PrivateKeyId" });
            DropIndex("dbo.webpages_UsersInRoles", new[] { "UserId" });
            DropIndex("dbo.webpages_UsersInRoles", new[] { "RoleId" });
            DropIndex("dbo.webpages_Membership", new[] { "UserId" });
            DropForeignKey("dbo.Licenses", "OwningCustomerId", "dbo.Customers");
            DropForeignKey("dbo.Licenses", "PurchasingCustomerId", "dbo.Customers");
            DropForeignKey("dbo.Licenses", "SkuId", "dbo.SKUs");
            DropForeignKey("dbo.Customers", "CountryCode", "dbo.Countries");
            DropForeignKey("dbo.Vendors", "CountryCode", "dbo.Countries");
            DropForeignKey("dbo.UserObjectRights", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserObjectRights", "RightId", "dbo.Rights");
            DropForeignKey("dbo.CustomerAppIssues", "CustomerAppId", "dbo.CustomerApps");
            DropForeignKey("dbo.CustomerAppKeys", "CustomerAppId", "dbo.CustomerApps");
            DropForeignKey("dbo.LicenseCustomerApps", "CustomerAppId", "dbo.CustomerApps");
            DropForeignKey("dbo.LicenseCustomerApps", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.TransactionIgnoredItems", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.TransactionItems", "SkuId", "dbo.SKUs");
            DropForeignKey("dbo.TransactionItems", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.TransactionItems", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.DomainLicenses", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.Features", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.SkuFeatures", "SkuId", "dbo.SKUs");
            DropForeignKey("dbo.SkuFeatures", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.PrivateKeys", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.SKUs", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.SKUs", "PrivateKeyId", "dbo.PrivateKeys");
            DropForeignKey("dbo.webpages_UsersInRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.webpages_UsersInRoles", "RoleId", "dbo.webpages_Roles");
            DropForeignKey("dbo.webpages_Membership", "UserId", "dbo.Users");
            DropTable("dbo.Licenses");
            DropTable("dbo.Customers");
            DropTable("dbo.Vendors");
            DropTable("dbo.webpages_OAuthMembership");
            DropTable("dbo.UserObjectRights");
            DropTable("dbo.Rights");
            DropTable("dbo.CustomerAppIssues");
            DropTable("dbo.CustomerAppKeys");
            DropTable("dbo.CustomerApps");
            DropTable("dbo.LicenseCustomerApps");
            DropTable("dbo.TransactionIgnoredItems");
            DropTable("dbo.Transactions");
            DropTable("dbo.TransactionItems");
            DropTable("dbo.DomainLicenses");
            DropTable("dbo.Features");
            DropTable("dbo.SkuFeatures");
            DropTable("dbo.PrivateKeys");
            DropTable("dbo.Countries");
            DropTable("dbo.SKUs");
            DropTable("dbo.webpages_Roles");
            DropTable("dbo.webpages_UsersInRoles");
            DropTable("dbo.Users");
            DropTable("dbo.webpages_Membership");
        }
    }
}
