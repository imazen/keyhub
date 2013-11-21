using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KeyHub.Model;
using System.Data.Entity;
using System.Linq;

namespace KeyHub.Data.Extensions
{
    public static class DomainLicenseExtensions
    {
        public static bool AnyEquals(this DbSet<DomainLicense> domainLicenseSet, DomainLicense domainLicense)
        {
            return domainLicenseSet.Any(x => x.DomainName == domainLicense.DomainName && x.LicenseId == domainLicense.LicenseId);
        }

        public static IQueryable<DomainLicense> AutomaticallyCreated(this IQueryable<DomainLicense> domainLicenses)
        {
            return domainLicenses.Where(x => x.AutomaticallyCreated);
        }

        public static IQueryable<DomainLicense> Expired(this IQueryable<DomainLicense> domainLicenses)
        {
            return domainLicenses.Where(x => x.DomainLicenseExpires < DateTime.Now);
        }
    }
}
