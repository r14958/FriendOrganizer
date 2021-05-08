using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess
{
    public class FriendOrganizerDbContextFactory
    {
        //private readonly string connectionString;
        private readonly Action<DbContextOptionsBuilder> configureDbContext;

        public FriendOrganizerDbContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            this.configureDbContext = configureDbContext;
        }

        public FriendOrganizerDbContext CreateDbContext()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<FriendOrganizerDbContext>();

            configureDbContext(optionsBuilder);

            return new FriendOrganizerDbContext(optionsBuilder.Options);
        }
    }
}
