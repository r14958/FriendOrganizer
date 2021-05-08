using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.DataAccess.Services
{
    public class FriendDataService : IFriendDataService
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public FriendDataService(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public IEnumerable<Friend> GetAll()
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            IEnumerable<Friend> friends = context.Friends
                .AsNoTracking()
                .ToList();

            return friends;
        }
    }
}
