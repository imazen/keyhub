using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Model;

namespace KeyHub.Data
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Membership tables
            modelBuilder.Entity<Membership>().HasRequired<User>(p => p.User);
            modelBuilder.Entity<User>().HasOptional<Profile>(p => p.Profile).WithRequired(x => x.User);

            modelBuilder.Entity<UserInRole>().ToTable("UsersInRoles");

            modelBuilder.Entity<Role>().HasMany(x => x.UsersInRoles).WithRequired(x => x.Role).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.UsersInRoles).WithRequired(x => x.User).WillCascadeOnDelete(false);
        }
    }
}