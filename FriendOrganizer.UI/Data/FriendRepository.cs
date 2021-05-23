using FriendOrganizer.DataAccess;
using FriendOrganizer.DataAccess.Services.Repositories;
using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class FriendRepository : DataRepositoryBase<Friend>
    {
        public FriendRepository(FriendOrganizerDbContextFactory contextFactory) : base(contextFactory) { }

    }
}
