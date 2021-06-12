using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FriendOrganizer.UITests.Wrapper
{
    [TestClass]
    public class BasicUnitTests
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
        public void ShouldContainModelModelInModelProperty()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.AreEqual(friend, wrapper.Model);
        }

        [TestMethod]
        public void ShouldThrowArgumentNullExceptionIfModelIsNull()
        {
            try
            {
                var wrapper = new FriendWrapper(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("model", ex.ParamName);
                throw;
            }
        }

        [TestMethod]
        public void ShouldGetValueOfUnderlyingModelProperty()
        {
            var wrapper = new FriendWrapper(friend);
            Assert.AreEqual(wrapper.Model.FirstName, wrapper.FirstName);
        }

        [TestMethod]
        public void ShouldSetValueOfUnderlyingModelProperty()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.FirstName = "Test2";
            Assert.AreEqual(wrapper.Model.FirstName, "Test2");
        }
    }
}
