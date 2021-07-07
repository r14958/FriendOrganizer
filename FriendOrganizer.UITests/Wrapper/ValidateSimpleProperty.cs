using FluentValidation;
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
    public class ValidateSimpleProperty
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
        public void ShouldReturnValidationErrorIfFirstNameIsEmptyUsingDataAnnotations()
        {
            // Do not pass in a FluentValidation validator, so relying only on data annotations
            // in the wrapper.
            var wrapper = new FriendWrapper(friend);
            Assert.IsFalse(wrapper.HasErrors);

            wrapper.FirstName = "";
            Assert.IsTrue(wrapper.HasErrors);

            var errors = wrapper.GetErrors(nameof(wrapper.FirstName)).Cast<string>();
            Assert.AreEqual(2, errors.Count());
            Assert.AreEqual("Please specify a first name.", errors.First());

            errors = wrapper.GetErrors(nameof(wrapper.FirstName)).Cast<string>();
            wrapper.FirstName = "J";
            Assert.AreEqual(1, errors.Count());
        }

        [TestMethod]
        public void ShouldReturnValidationErrorIfFirstNameOrLastNameIsEmpty()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.IsFalse(wrapper.HasErrors);

            wrapper.FirstName = "";
            Assert.IsTrue(wrapper.HasErrors);

            var errors = wrapper.GetErrors(nameof(wrapper.FirstName)).Cast<string>();
            Assert.AreEqual(2, errors.Count());
            Assert.AreEqual("Please specify a first name.", errors.First());

            wrapper.FirstName = "J";
            Assert.IsTrue(wrapper.HasErrors);

            errors = wrapper.GetErrors(nameof(wrapper.FirstName)).Cast<string>();
            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("First name must be at least 2 characters.", errors.First());

            wrapper.FirstName = "Linda";
            wrapper.LastName = "";
            Assert.IsTrue(wrapper.HasErrors);

            errors = wrapper.GetErrors(nameof(wrapper.LastName)).Cast<string>();
            Assert.AreEqual(2, errors.Count());
            Assert.AreEqual("Please specify a last name.", errors.First());

            wrapper.LastName = "S";
            Assert.IsTrue(wrapper.HasErrors);

            errors = wrapper.GetErrors(nameof(wrapper.LastName)).Cast<string>();
            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Last name must be at least 2 characters.", errors.First());

            wrapper.LastName = "Smith";
            Assert.IsFalse(wrapper.HasErrors);

        }

        [TestMethod]
        public void ShouldRaiseErrorsChangedEventWhenFirstNameIsSetToEmptyAndBack()
        {
            var fired = false;
            var wrapper = new FriendWrapper(friend);

            wrapper.ErrorsChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.FirstName))
                {
                    fired = true;
                }
            };

            wrapper.FirstName = "";
            Assert.IsTrue(fired);

            fired = false;
            wrapper.FirstName = "Linda";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldSetHasErrorsAndIsValid()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.IsFalse(wrapper.HasErrors);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.FirstName = "";
            Assert.IsTrue(wrapper.HasErrors);
            Assert.IsFalse(wrapper.IsValid);

            wrapper.FirstName = "Linda";
            Assert.IsFalse(wrapper.HasErrors);
            Assert.IsTrue(wrapper.IsValid);
        }

        [TestMethod]
        public void ShouldRaisePropertyChangedEventFoIsValid()
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

            wrapper.FirstName = "";
            Assert.IsTrue(fired);

            fired = false;
            wrapper.FirstName = "Linda";
            Assert.IsTrue(fired);
        }

       [TestMethod]
        public void ShouldSetErrorsAndIsValidAfterWrapperInitialization()
        {
            friend.FirstName = "";
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.HasErrors);
            Assert.IsFalse(wrapper.IsValid);

            var errors = wrapper.GetErrors(nameof(wrapper.FirstName)).Cast<string>();
            Assert.AreEqual(2, errors.Count());
            Assert.AreEqual("Please specify a first name.", errors.First());
        }

        [TestMethod]
        public void ShouldRefreshErrorsAndIsValidWhenRejectingChanges()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.IsValid);
            Assert.IsFalse(wrapper.HasErrors);

            wrapper.FirstName = "";
            Assert.IsFalse(wrapper.IsValid);
            Assert.IsTrue(wrapper.HasErrors);

            wrapper.RejectChanges();
            Assert.IsTrue(wrapper.IsValid);
            Assert.IsFalse(wrapper.HasErrors);
        }
    }
}
