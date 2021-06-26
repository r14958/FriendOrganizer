using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FriendOrganizer.UITests.Wrapper
{
    [TestClass]
    public class ChangeNotificationComplexPropertyClass
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new Friend
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Address = new(),
                PhoneNumbers = new List<FriendPhoneNumber>()
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionIfAddressIsNull()
        {
            try
            {
                friend.Address = null;
                var wrapper = new FriendWrapper(friend);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Address cannot be null.", ex.Message);
                throw;
            }
        }

    }

}
