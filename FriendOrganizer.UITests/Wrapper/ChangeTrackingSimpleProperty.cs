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
    public class ChangeTrackingSimpleProperty
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
                PhoneNumbers = new List<FriendPhoneNumber>()
            };
        }

        [TestMethod]
        public void ShouldStoreOriginalValue()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.AreEqual("Thomas", wrapper.FirstNameOriginalValue);
            wrapper.FirstName = "Bob";
            Assert.AreEqual("Thomas", wrapper.FirstNameOriginalValue);
            Assert.AreNotEqual(wrapper.FirstName, wrapper.FirstNameOriginalValue);
        }

        [TestMethod]
        public void ShouldSetIsChanged()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.FirstName = "Thomas";
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
            
            wrapper.FirstName = "Bob";
            Assert.IsTrue(wrapper.FirstNameIsChanged);
            Assert.IsTrue(wrapper.IsChanged);
            
            wrapper.FirstName = "Thomas";
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventFirstNameIsChanged()
        {
            bool fired = false;
            var wrapper = new FriendWrapper(friend);
            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.FirstNameIsChanged))
                {
                    fired = true; 
                }
            };
            // Change the value of FirstName.
            wrapper.FirstName = "Julia";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventIsChanged()
        {
            bool fired = false;
            var wrapper = new FriendWrapper(friend);
            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.IsChanged))
                {
                    fired = true;
                }
            };
            // Change the value of FirstName.
            wrapper.FirstName = "Julia";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.FirstName = "Bob";
            Assert.AreEqual("Bob", wrapper.FirstName);
            Assert.AreEqual("Thomas", wrapper.FirstNameOriginalValue);
            Assert.IsTrue(wrapper.FirstNameIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.AreEqual("Bob", wrapper.FirstName);
            Assert.AreEqual("Bob", wrapper.FirstNameOriginalValue);
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.FirstName = "Bob";
            Assert.AreEqual("Bob", wrapper.FirstName);
            Assert.AreEqual("Thomas", wrapper.FirstNameOriginalValue);
            Assert.IsTrue(wrapper.FirstNameIsChanged);
            Assert.IsTrue(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.AreEqual("Thomas", wrapper.FirstName);
            Assert.AreEqual("Thomas", wrapper.FirstNameOriginalValue);
            Assert.IsFalse(wrapper.FirstNameIsChanged);
            Assert.IsFalse(wrapper.IsChanged);
        }
    }
}
