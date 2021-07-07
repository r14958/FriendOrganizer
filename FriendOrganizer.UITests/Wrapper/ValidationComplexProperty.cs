using FriendOrganizer.Domain.Models;
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
    public class ValidationComplexProperty
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new Friend
            {
                FirstName = "Jeff",
                LastName = "Klein",
                Address = new Address { City = "Boxford" },
                PhoneNumbers = new List<FriendPhoneNumber>()
            };
        }

        [TestMethod]
        public void ShouldSetIsValidRoot()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.Address.City = "";
            Assert.IsFalse(wrapper.IsValid);

            wrapper.Address.City = "Boston";
            Assert.IsTrue(wrapper.IsValid);
        }

        [TestMethod]
        public void ShouldSetIsValidOfRootAfterInitialization()
        {
            friend.Address.City = "";
            var wrapper = new FriendWrapper(friend);
            Assert.IsFalse(wrapper.IsValid);

            wrapper.Address.City = "Boston";
            Assert.IsTrue(wrapper.IsValid);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventForIsValidOfRoot()
        {
            var fired = false;
            var wrapper = new FriendWrapper(friend);

            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.IsValid))
                {
                    fired = true;
                }
            };

            wrapper.Address.City = "";
            Assert.IsTrue(fired);

            fired = false;
            wrapper.Address.City = "Boston";
            Assert.IsTrue(fired);
        }
    }
}
