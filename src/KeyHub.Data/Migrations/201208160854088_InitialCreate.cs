namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration, IMigrationDataSeeder<DataContext>
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        ApplicationId = c.Guid(nullable: false),
                        ApplicationName = c.String(nullable: false, maxLength: 235),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Memberships",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        Password = c.String(nullable: false, maxLength: 128),
                        PasswordFormat = c.Int(nullable: false),
                        PasswordSalt = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        PasswordQuestion = c.String(maxLength: 256),
                        PasswordAnswer = c.String(maxLength: 128),
                        IsApproved = c.Boolean(nullable: false),
                        IsLockedOut = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        LastPasswordChangedDate = c.DateTime(nullable: false),
                        LastLockoutDate = c.DateTime(nullable: false),
                        FailedPasswordAttemptCount = c.Int(nullable: false),
                        FailedPasswordAttemptWindowStart = c.DateTime(nullable: false),
                        FailedPasswordAnswerAttemptCount = c.Int(nullable: false),
                        FailedPasswordAnswerAttemptWindowsStart = c.DateTime(nullable: false),
                        Comment = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.ApplicationId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 50),
                        IsAnonymous = c.Boolean(nullable: false),
                        LastActivityDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        PropertyNames = c.String(nullable: false, maxLength: 4000),
                        PropertyValueStrings = c.String(nullable: false, maxLength: 4000),
                        PropertyValueBinary = c.Binary(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UsersInRoles",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        RoleName = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.OpenAuthUsers",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        OpenAuthIdentifier = c.String(nullable: false, maxLength: 1024),
                    })
                .PrimaryKey(t => new { t.UserId, t.OpenAuthIdentifier })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
                "dbo.Features",
                c => new
                    {
                        FeatureId = c.Guid(nullable: false, identity: true),
                        VendorId = c.Guid(nullable: false),
                        FeatureCode = c.String(nullable: false, maxLength: 256, unicode: false),
                    })
                .PrimaryKey(t => t.FeatureId)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.PrivateKeys",
                c => new
                    {
                        PrivateKeyId = c.Guid(nullable: false, identity: true),
                        VendorId = c.Guid(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 256, unicode: false),
                        KeyBytes = c.Binary(nullable: false, maxLength: 4096),
                    })
                .PrimaryKey(t => t.PrivateKeyId)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.SKUs",
                c => new
                    {
                        SkuId = c.Guid(nullable: false, identity: true),
                        VendorId = c.Guid(nullable: false),
                        PrivateKeyId = c.Guid(nullable: false),
                        SkuCode = c.String(nullable: false, maxLength: 256, unicode: false),
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
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .ForeignKey("dbo.PrivateKeys", t => t.PrivateKeyId)
                .Index(t => t.VendorId)
                .Index(t => t.PrivateKeyId);
            
            CreateTable(
                "dbo.SkuFeatures",
                c => new
                    {
                        SkuId = c.Guid(nullable: false),
                        FeatureId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SkuId, t.FeatureId })
                .ForeignKey("dbo.SKUs", t => t.SkuId)
                .ForeignKey("dbo.Features", t => t.FeatureId, cascadeDelete: true)
                .Index(t => t.SkuId)
                .Index(t => t.FeatureId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.TransactionId);
            
            CreateTable(
                "dbo.TransactionItems",
                c => new
                    {
                        TransactionItemId = c.Int(nullable: false, identity: true),
                        TransactionId = c.Int(nullable: false),
                        LicenseId = c.Guid(),
                        SkuId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionItemId)
                .ForeignKey("dbo.Licenses", t => t.LicenseId)
                .ForeignKey("dbo.SKUs", t => t.SkuId, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.TransactionId)
                .Index(t => t.LicenseId)
                .Index(t => t.SkuId)
                .Index(t => t.TransactionId);
            
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
                        KeyBytes = c.Binary(nullable: false, maxLength: 4096),
                    })
                .PrimaryKey(t => t.DomainLicenseId)
                .ForeignKey("dbo.Licenses", t => t.LicenseId, cascadeDelete: true)
                .Index(t => t.LicenseId);
            
            CreateTable(
                "dbo.Rights",
                c => new
                    {
                        RightId = c.Guid(nullable: false, identity: true),
                        DisplayName = c.String(maxLength: 256, unicode: false),
                    })
                .PrimaryKey(t => t.RightId);
            
            CreateTable(
                "dbo.UserObjectRights",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ObjectId = c.Guid(nullable: false),
                        RightId = c.Guid(nullable: false),
                        ObjectType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ObjectId, t.RightId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Rights", t => t.RightId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RightId);
            
            CreateTable(
                "dbo.CustomerApps",
                c => new
                    {
                        CustomerAppId = c.Guid(nullable: false, identity: true),
                        ApplicationName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.CustomerAppId);
            
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
                "dbo.CustomerAppKeys",
                c => new
                    {
                        CustomerAppKeyId = c.Int(nullable: false, identity: true),
                        CustomerAppId = c.Guid(nullable: false),
                        AppKey = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerAppKeyId)
                .ForeignKey("dbo.CustomerApps", t => t.CustomerAppId, cascadeDelete: true)
                .Index(t => t.CustomerAppId);
            
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
                        Department = c.String(nullable: false, maxLength: 512),
                        Street = c.String(nullable: false, maxLength: 512),
                        PostalCode = c.String(nullable: false, maxLength: 24),
                        City = c.String(nullable: false, maxLength: 256),
                        Region = c.String(nullable: false, maxLength: 256),
                        CountryCode = c.String(nullable: false, maxLength: 12, unicode: false),
                        PayPalId = c.String(maxLength: 256),
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
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Licenses", new[] { "OwningCustomerId" });
            DropIndex("dbo.Licenses", new[] { "PurchasingCustomerId" });
            DropIndex("dbo.Licenses", new[] { "SkuId" });
            DropIndex("dbo.Customers", new[] { "CountryCode" });
            DropIndex("dbo.Vendors", new[] { "CountryCode" });
            DropIndex("dbo.CustomerAppKeys", new[] { "CustomerAppId" });
            DropIndex("dbo.LicenseCustomerApps", new[] { "CustomerAppId" });
            DropIndex("dbo.LicenseCustomerApps", new[] { "LicenseId" });
            DropIndex("dbo.UserObjectRights", new[] { "RightId" });
            DropIndex("dbo.UserObjectRights", new[] { "UserId" });
            DropIndex("dbo.DomainLicenses", new[] { "LicenseId" });
            DropIndex("dbo.TransactionItems", new[] { "TransactionId" });
            DropIndex("dbo.TransactionItems", new[] { "SkuId" });
            DropIndex("dbo.TransactionItems", new[] { "LicenseId" });
            DropIndex("dbo.SkuFeatures", new[] { "FeatureId" });
            DropIndex("dbo.SkuFeatures", new[] { "SkuId" });
            DropIndex("dbo.SKUs", new[] { "PrivateKeyId" });
            DropIndex("dbo.SKUs", new[] { "VendorId" });
            DropIndex("dbo.PrivateKeys", new[] { "VendorId" });
            DropIndex("dbo.Features", new[] { "VendorId" });
            DropIndex("dbo.OpenAuthUsers", new[] { "UserId" });
            DropIndex("dbo.Roles", new[] { "ApplicationId" });
            DropIndex("dbo.UsersInRoles", new[] { "UserId" });
            DropIndex("dbo.UsersInRoles", new[] { "RoleId" });
            DropIndex("dbo.Profiles", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "ApplicationId" });
            DropIndex("dbo.Memberships", new[] { "UserId" });
            DropIndex("dbo.Memberships", new[] { "ApplicationId" });
            DropForeignKey("dbo.Licenses", "OwningCustomerId", "dbo.Customers");
            DropForeignKey("dbo.Licenses", "PurchasingCustomerId", "dbo.Customers");
            DropForeignKey("dbo.Licenses", "SkuId", "dbo.SKUs");
            DropForeignKey("dbo.Customers", "CountryCode", "dbo.Countries");
            DropForeignKey("dbo.Vendors", "CountryCode", "dbo.Countries");
            DropForeignKey("dbo.CustomerAppKeys", "CustomerAppId", "dbo.CustomerApps");
            DropForeignKey("dbo.LicenseCustomerApps", "CustomerAppId", "dbo.CustomerApps");
            DropForeignKey("dbo.LicenseCustomerApps", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.UserObjectRights", "RightId", "dbo.Rights");
            DropForeignKey("dbo.UserObjectRights", "UserId", "dbo.Users");
            DropForeignKey("dbo.DomainLicenses", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.TransactionItems", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.TransactionItems", "SkuId", "dbo.SKUs");
            DropForeignKey("dbo.TransactionItems", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.SkuFeatures", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.SkuFeatures", "SkuId", "dbo.SKUs");
            DropForeignKey("dbo.SKUs", "PrivateKeyId", "dbo.PrivateKeys");
            DropForeignKey("dbo.SKUs", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.PrivateKeys", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.Features", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.OpenAuthUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Roles", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.UsersInRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersInRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Profiles", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.Memberships", "UserId", "dbo.Users");
            DropForeignKey("dbo.Memberships", "ApplicationId", "dbo.Applications");
            DropTable("dbo.Licenses");
            DropTable("dbo.Customers");
            DropTable("dbo.Vendors");
            DropTable("dbo.CustomerAppKeys");
            DropTable("dbo.LicenseCustomerApps");
            DropTable("dbo.CustomerApps");
            DropTable("dbo.UserObjectRights");
            DropTable("dbo.Rights");
            DropTable("dbo.DomainLicenses");
            DropTable("dbo.TransactionItems");
            DropTable("dbo.Transactions");
            DropTable("dbo.SkuFeatures");
            DropTable("dbo.SKUs");
            DropTable("dbo.PrivateKeys");
            DropTable("dbo.Features");
            DropTable("dbo.Countries");
            DropTable("dbo.OpenAuthUsers");
            DropTable("dbo.Roles");
            DropTable("dbo.UsersInRoles");
            DropTable("dbo.Profiles");
            DropTable("dbo.Users");
            DropTable("dbo.Memberships");
            DropTable("dbo.Applications");
        }

        public void Seed(DataContext context)
        {
            // Get all countries from the framework and insert them into the table
            foreach (var country in Common.Utils.Globalization.Countries.GetAllCountries())
            {
                context.Countries.Add(new Model.Country()
                {
                    CountryCode = country.CountryCode,
                    CountryName = country.CountryName,
                    NativeCountryName = country.NativeCountryName
                });
            }
        }
    }
}
