using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FriendOrganizer.UITests.Wrapper
{
    [TestClass]
    public class ChangeTrackingComplexProperty
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new Friend
            {
                FirstName = "Bob",
                LastName = "Smith",
                Address = new Address { City = "Cleveland", },
                PhoneNumbers = new List<FriendPhoneNumber>()
            };
        }

        [TestMethod]
        public void ShouldSetIsChangedOfFriendWrapper()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.Address.City = "Toledo";
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.Address.City = "Cleveland";
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventOIsChangedOfFriendWrapper()
        {
            bool fired = false;
            var wrapper = new FriendWrapper(friend);
            wrapper.PropertyChanged += (s, e) =>
            {
                var name = nameof(wrapper.IsChanged);
                if (e.PropertyName == name)
                {
                    fired = true;
                }
            };
            // Change the value of FirstName.
            wrapper.Address.City = "Toledo";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.Address.City = "Toledo";
            Assert.AreEqual("Toledo", wrapper.Address.City);
            Assert.AreEqual("Cleveland", wrapper.Address.CityOriginalValue);
            Assert.IsTrue(wrapper.Address.CityIsChanged);
            Assert.IsTrue(wrapper.Address.IsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.AreEqual("Toledo", wrapper.Address.City);
            Assert.AreEqual("Toledo", wrapper.Address.CityOriginalValue);
            Assert.IsFalse(wrapper.Address.CityIsChanged);
            Assert.IsFalse(wrapper.Address.IsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.Address.City = "Toledo";
            Assert.AreEqual("Toledo", wrapper.Address.City);
            Assert.AreEqual("Cleveland", wrapper.Address.CityOriginalValue);
            Assert.IsTrue(wrapper.Address.CityIsChanged);
            Assert.IsTrue(wrapper.Address.IsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual("Cleveland", wrapper.Address.City);
            Assert.AreEqual("Cleveland", wrapper.Address.CityOriginalValue);
            Assert.IsFalse(wrapper.Address.CityIsChanged);
            Assert.IsFalse(wrapper.Address.IsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }
    }
}
