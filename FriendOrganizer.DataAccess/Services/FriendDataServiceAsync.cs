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
    public class FriendDataServiceAsync : IFriendDataServiceAsync
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public FriendDataServiceAsync(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<Friend> GetByIdAsync(int friendId)
        {
            // Create a disposable DataContext.
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            
            // Use the context to find the target Friend.
            // Note that this will throw an exception if not found.
            Friend friend = await context.Friends
                .AsNoTracking()
                .SingleAsync(f => f.Id == friendId);

            // Return the found Friend
            return friend;
        }

        public async Task SaveAsync(Friend friend)
        {
            // Create a disposable DataContext.
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();

            // Make the context aware of the modified entity.
            context.Friends.Attach(friend);

            //Notify the context that the entity has been modified.
            context.Entry(friend).State = EntityState.Modified;

            // Save changes to the DB.
            await context.SaveChangesAsync();
        }
    }
}
