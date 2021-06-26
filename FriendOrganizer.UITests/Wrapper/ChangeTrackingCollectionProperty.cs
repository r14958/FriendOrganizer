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
    public class ChangeTrackingCollectionProperty
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new Friend
            {
                FirstName = "Thomas",
                LastName = "Huber",
                Address = new(),
                PhoneNumbers = new List<FriendPhoneNumber>
                {
                    new FriendPhoneNumber {Number="111-1111"},
                    new FriendPhoneNumber {Number="222-2222"},
                }
            };
        }

        [TestMethod]
        public void ShouldSetIsChangedOfFriendWrapper()
        {
            var wrapper = new FriendWrapper(friend);
            var phoneToModify = wrapper.PhoneNumbers.First();
            phoneToModify.Number = "333-3333";

            Assert.IsTrue(wrapper.IsChanged);

            phoneToModify.Number = "111-1111";
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
            wrapper.PhoneNumbers.First().Number = "333-3333";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.PhoneNumbers.First().Number = "333-3333";
            Assert.AreEqual("333-3333", wrapper.PhoneNumbers.First().Number);
            Assert.AreEqual("111-1111", wrapper.PhoneNumbers.First().NumberOriginalValue);
            Assert.IsTrue(wrapper.PhoneNumbers.First().NumberIsChanged);
            Assert.IsTrue(wrapper.PhoneNumbers.IsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.AreEqual("333-3333", wrapper.PhoneNumbers.First().Number);
            Assert.AreEqual("333-3333", wrapper.PhoneNumbers.First().NumberOriginalValue);
            Assert.IsFalse(wrapper.PhoneNumbers.First().NumberIsChanged);
            Assert.IsFalse(wrapper.PhoneNumbers.IsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.PhoneNumbers.First().Number = "333-3333";
            Assert.AreEqual("333-3333", wrapper.PhoneNumbers.First().Number);
            Assert.AreEqual("111-1111", wrapper.PhoneNumbers.First().NumberOriginalValue);
            Assert.IsTrue(wrapper.PhoneNumbers.First().NumberIsChanged);
            Assert.IsTrue(wrapper.PhoneNumbers.IsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual("111-1111", wrapper.PhoneNumbers.First().Number);
            Assert.AreEqual("111-1111", wrapper.PhoneNumbers.First().NumberOriginalValue);
            Assert.IsFalse(wrapper.PhoneNumbers.First().NumberIsChanged);
            Assert.IsFalse(wrapper.PhoneNumbers.IsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

    }
}
