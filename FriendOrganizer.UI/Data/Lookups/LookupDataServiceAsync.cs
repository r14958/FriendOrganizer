using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services.Lookups

{
    /// <summary>
    /// Generic data service to return a read-only, lightweight, IEnumerable collection of two properties of an <see cref="EntityBase"/> data model: the item's id
    /// and one other property that must return a string.
    /// </summary>
    /// <typeparam name="T">The type of the (<see cref="EntityBase"/> data model.</typeparam>
    public class LookupDataServiceAsync<T> : ILookupDataService<T> where T : EntityBase
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;
        private readonly string propertyName;

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="contextFactory">The <see cref="FriendOrganizerDbContextFactory"/> that provides the DbContext for EF Core.</param>
        /// <param name="propertyName">The name of the property that will be returned.  Must return a string.</param>
        public LookupDataServiceAsync(FriendOrganizerDbContextFactory contextFactory, string propertyName)
        {
            this.contextFactory = contextFactory;
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Transforms a collection of Db entities of type T into a list of lightweight lookup items of type T.
        /// </summary>
        /// <returns>A list of <see cref="LookupItem{T}"/></returns>
        public async Task<IEnumerable<LookupItem<T>>> GetLookupAsync()
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            // Cycle through each T entity in the DbSet<T>, transforming each into a Lookup item, and return them as a List.
            return await context.Set<T>()
                .AsNoTracking()
                .Select(entity =>
                new LookupItem<T>
                {
                    Id = entity.Id,
                    DisplayMember = typeof(T).GetProperty(propertyName).GetValue(entity).ToString()
                }).ToListAsync();
        }
        
    }
}
