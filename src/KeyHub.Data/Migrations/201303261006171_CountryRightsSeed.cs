using System.Collections.Generic;
using KeyHub.Model;

namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CountryRightsSeed : DbMigration, IMigrationDataSeeder<DataContext>
    {
        public override void Up()
        {
        }
        
        public override void Down()
        {
        }

        public void Seed(DataContext context)
        {
            // Get all countries from the framework and insert them into the table
            foreach (var country in Common.Utils.Globalization.Countries.GetAllCountries())
            {
                context.Countries.Add(new Country
                {
                    CountryCode = country.CountryCode,
                    CountryName = country.CountryName,
                    NativeCountryName = country.NativeCountryName
                });
            }

            // Get all countries from the framework and insert them into the table
            foreach (var right in GetRights())
            {
                context.Rights.Add(new Right { DisplayName = right.DisplayName, RightId = right.RightId });
            }
        }

        private static IEnumerable<IRight> GetRights()
        {
            yield return new BelongToEntity();
            yield return new EditEntityInfo();
            yield return new EditEntityMembers();
            yield return new EditLicenseInfo();
            yield return new GrantMemberRights();
            yield return new VendorAdmin();
            yield return new VendorReporting();
            yield return new ViewLicenseInfo();
        }
    }
}
