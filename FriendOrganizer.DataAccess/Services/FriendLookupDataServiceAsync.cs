using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services
{
    public class FriendLookupDataServiceAsync : IFriendLookupDataServiceAsync
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public FriendLookupDataServiceAsync(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            return await context.Friends
                .AsNoTracking()
                .Select(f =>
                new LookupItem
                {
                    Id = f.Id,
                    DisplayMember = f.FullName
                }).ToListAsync();
        }
        
    }
}
