﻿using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public interface IViewModel<T> where T : EntityBase
    {
        Task LoadAsync(int? id);

        public bool HasChanges { get; set; }
    }
}