using FriendOrganizer.DataAccess;
using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class ProgrammingLanguageRepository
        : GenericRepository<ProgrammingLanguage>,
        IProgrammingLanguageRepository
    {
        public ProgrammingLanguageRepository(FriendOrganizerDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        /// <summary>
        /// Checks whether any <see cref="Friend"/> entity references a <see cref="ProgrammingLanguage"/> with the target Id.
        /// </summary>
        /// <param name="programmingLanguadeId">Id of the target <see cref="ProgrammingLanguage"/>.</param>
        /// <returns>True if any <see cref="Friend"/> references the <see cref="ProgrammingLanguage"/>; otherwise, <see cref="false"/>.</returns>
        public Task<bool> IsReferencedByFriendAync(int programmingLanguadeId)
        {
            return context.Friends.AsNoTracking().AnyAsync(f => f.FavoriteLanguageId == programmingLanguadeId);
        }
    }
}
