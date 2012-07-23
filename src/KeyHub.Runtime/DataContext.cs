using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Runtime
{
    /// <summary>
    /// Provides data access to all tables and collections
    /// </summary>
    public class DataContext : DbContext
    {
        public DbSet<Application> Applications { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<OpenAuthUser> OpenAuthUsers { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<PrivateKey> PrivateKeys { get; set; }

        public DbSet<SKU> SKUs { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Membership tables
            modelBuilder.Entity<Membership>().HasRequired<User>(p => p.User);
            modelBuilder.Entity<User>().HasOptional<Profile>(p => p.Profile).WithRequired(x => x.User);

            modelBuilder.Entity<UserInRole>().ToTable("UsersInRoles");

            modelBuilder.Entity<Role>().HasMany(x => x.UsersInRoles).WithRequired(x => x.Role).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.UsersInRoles).WithRequired(x => x.User).WillCascadeOnDelete(true);

            // Keyhub tables
            modelBuilder.Entity<PrivateKey>().HasMany(x => x.SKUs).WithRequired(x => x.PrivateKey).WillCascadeOnDelete(false);

            modelBuilder.Entity<SkuFeature>().ToTable("SkuFeatures");

            modelBuilder.Entity<SKU>().HasMany(x => x.SkuFeatures).WithRequired(x => x.Sku).WillCascadeOnDelete(false);
            modelBuilder.Entity<Feature>().HasMany(x => x.SkuFeatures).WithRequired(x => x.Feature).WillCascadeOnDelete(true);
        }
    }
}