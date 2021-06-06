﻿using FriendOrganizer.UI.ViewModel;
using FriendOrganizer.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;
    public delegate IDetailViewModel CreateDetailViewModel(string viewModelName);

    public static class Utilities
    {
        public enum ViewType
        {
            FriendDetail,
            MeetingDetail,
            Navigation

        }

        
    }
}
