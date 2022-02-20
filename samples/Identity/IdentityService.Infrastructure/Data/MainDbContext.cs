using System;
using System.Collections.Generic;
using IdentityService.AppCore.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using N8T.Core.Domain;
using N8T.Infrastructure.EfCore;

namespace IdentityService.Infrastructure.Data
{
    public class MainDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>,
        IDbFacadeResolver,
        IDomainEventContext
    {
        private const string Schema = "user";

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension(Constants.UuidGenerator);

            // ApplicationUser
            modelBuilder.Entity<ApplicationUser>().ToTable("user", Schema);
            modelBuilder.Entity<ApplicationUser>().HasKey(x => x.Id);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Id).HasColumnType("text");

            modelBuilder.Entity<ApplicationUser>().Property(x => x.UserName).HasColumnName("user_name");
            modelBuilder.Entity<ApplicationUser>().Property(x => x.FirstName).HasColumnName("first_name");
            modelBuilder.Entity<ApplicationUser>().Property(x => x.LastName).HasColumnName("last_name");
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Role).HasColumnName("role");
            modelBuilder.Entity<ApplicationUser>().Property(x => x.Email).HasColumnName("email");

            modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.Id).IsUnique();
            modelBuilder.Entity<ApplicationUser>().Ignore(x => x.FullName);
        }

        public IEnumerable<EventBase> GetDomainEvents()
        {
            return new List<EventBase>();
        }
    }
}
