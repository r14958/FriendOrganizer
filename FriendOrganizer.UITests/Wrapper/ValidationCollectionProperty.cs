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
    public class ValidationCollectionProperty
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new Friend
            {
                Id = 1,
                FirstName = "Jeff",
                LastName = "Klein",
                Address = new Address { City = "Boxford" },
                PhoneNumbers = new List<FriendPhoneNumber>
                {
                    new FriendPhoneNumber { Number = "978.555.5555", FriendId = 1},
                    new FriendPhoneNumber { Number = "203.555.5555", FriendId = 1},

                }
            };
        }

        [TestMethod]
        public void ShouldSetIsValidOfRoot()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.PhoneNumbers.First().Number = "555";
            Assert.IsFalse(wrapper.IsValid);

            wrapper.PhoneNumbers.First().Number = "978.555.5555";
            Assert.IsTrue(wrapper.IsValid);
        }

        [TestMethod]
        public void ShouldSetIsValidOfRootWhenInitializing()
        {
            friend.PhoneNumbers.First().Number = "555";
            var wrapper = new FriendWrapper(friend);
            Assert.IsFalse(wrapper.IsValid);
            Assert.IsFalse(wrapper.HasErrors);
            Assert.IsTrue(wrapper.PhoneNumbers.First().HasErrors);
        }

        [TestMethod]
        public void ShouldSetIsValidOfRootWhenRemovingInvalidItem()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.PhoneNumbers.First().Number = "555";
            Assert.IsFalse(wrapper.IsValid);

            wrapper.PhoneNumbers.Remove(wrapper.PhoneNumbers.First());
            Assert.IsTrue(wrapper.IsValid);
        }

        [TestMethod]
        public void ShouldSetIsValidOfRootWhenAddingInvalidItem()
        {
            var phoneToAdd = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.PhoneNumbers.Add(phoneToAdd);
            Assert.IsFalse(wrapper.IsValid);

            phoneToAdd.Number = "978.555.5555";
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

            wrapper.PhoneNumbers.First().Number = "555";
            Assert.IsTrue(fired);

            fired = false;
            wrapper.PhoneNumbers.First().Number = "978.555.5555";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventForIsValidOfRootWhenRemovingInvalidItem()
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

            wrapper.PhoneNumbers.First().Number = "555";
            Assert.IsTrue(fired);

            fired = false;
            wrapper.PhoneNumbers.Remove(wrapper.PhoneNumbers.First());
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventForIsValidOfRootWhenAddingInvalidItem()
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

            var phoneToAdd = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            wrapper.PhoneNumbers.Add(phoneToAdd);
            Assert.IsTrue(fired);

            fired = false;
            phoneToAdd.Number = "978.555.5555";
            Assert.IsTrue(fired);
        }
    }
}
