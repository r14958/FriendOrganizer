using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.DataAccess
{
    public class FriendOrganizerDbContext : DbContext
    {
        public FriendOrganizerDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Friend> Friends { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=FriendsOrganizer.db");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Created custom extension method for modelBuilder to prevent naming DB tables as the pluralized
            modelBuilder.RemovePluralizingTableNameConvention();

            // Apply configuration from all instances of IEntityTypeConfiguration that are in the same assembly
            // (project) of this DbContext inside the appropriate folder. 
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FriendOrganizerDbContext).Assembly,
                x => x.Namespace.Contains(nameof(EntityConfiguration)));
            modelBuilder.Seed();
           
        }
    }

}
