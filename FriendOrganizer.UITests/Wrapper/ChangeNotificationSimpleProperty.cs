﻿using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UITests.Wrapper
{
    [TestClass]   
    public class ChangeNotificationSimpleProperty
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new Friend
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName"
            };
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventOnPropertyChange()
        {
            bool fired = false;
            var wrapper = new FriendWrapper(friend);
            wrapper.PropertyChanged += (s, e) => 
            {
                fired = e.PropertyName == "FirstName";
            };
            // Change the value of FirstName.
            wrapper.FirstName = "NewTestFirstName";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldNotRaisePropertyChangedEventIfPropertyIsSetToSameValue()
        {
            bool fired = false;
            var wrapper = new FriendWrapper(friend);
            wrapper.PropertyChanged += (s, e) =>
            {
                fired = e.PropertyName == "FirstName";
            };
            // Do not change the value of FirstName
            wrapper.FirstName = wrapper.FirstName;
            Assert.IsFalse(fired);
        }
    }
}
