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
    public class ValidationClassLevel
    {
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friend = new()
            {
                FirstName = "Jeff",
                LastName = "Klein",
                Address = new() { City = "Boxford" },
                IsDeveloper = false,
                FavoriteLanguageId = 0
            };
        }

        [TestMethod]
        public void ShouldHaveErrorsAndNotBeValidWhenIsDeveloperIsTrueAndNoFavoriteLanguageExists()
        {
            var expectedError = "Please select a favorite language for the developer.";

            var wrapper = new FriendWrapper(friend);
            wrapper.FavoriteLanguageId = null;
            Assert.IsFalse(wrapper.IsDeveloper);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.IsDeveloper = true;
            Assert.IsFalse(wrapper.IsValid);

            var languageErrors = wrapper.GetErrors(nameof(wrapper.FavoriteLanguageId)).Cast<string>().ToList();
            Assert.AreEqual(1, languageErrors.Count);
            Assert.AreEqual(expectedError, languageErrors.Single());

            var isDeveloperErrors = wrapper.GetErrors(nameof(wrapper.IsDeveloper)).Cast<string>().ToList();
            Assert.AreEqual(1, isDeveloperErrors.Count);
            Assert.AreEqual(expectedError, isDeveloperErrors.Single());
        }

        [TestMethod]
        public void ShouldBeValidAgainWhenIsDeveloperisSetBackToFalse()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.FavoriteLanguageId = null;
            Assert.IsFalse(wrapper.IsDeveloper);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.IsDeveloper = true;
            Assert.IsFalse(wrapper.IsValid);

            wrapper.IsDeveloper = false;
            Assert.IsTrue(wrapper.IsValid);

            var languageErrors = wrapper.GetErrors(nameof(wrapper.FavoriteLanguageId))?.Cast<string>().ToList() ?? new List<string>();
            Assert.AreEqual(0, languageErrors.Count);

            var isDeveloperErrors = wrapper.GetErrors(nameof(wrapper.IsDeveloper))?.Cast<string>().ToList() ?? new List<string>();
            Assert.AreEqual(0, isDeveloperErrors.Count);
        }

        [TestMethod]
        public void ShouldBeValidAgainWhenFavoriteLanguageIsAdded()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.FavoriteLanguageId = null;
            Assert.IsFalse(wrapper.IsDeveloper);
            Assert.IsTrue(wrapper.IsValid);

            wrapper.IsDeveloper = true;
            Assert.IsFalse(wrapper.IsValid);

            wrapper.FavoriteLanguageId = 1;
            Assert.IsTrue(wrapper.IsValid);

            // Must test if the GetErrors property returns null.
            var languageErrors = wrapper.GetErrors(nameof(wrapper.FavoriteLanguageId))?.Cast<string>().ToList() ?? new List<string>();
            Assert.AreEqual(0, languageErrors.Count);

            // Must test if the GetErrors property returns null.
            var isDeveloperErrors = wrapper.GetErrors(nameof(wrapper.IsDeveloper))?.Cast<string>().ToList() ?? new List<string>();
            Assert.AreEqual(0, isDeveloperErrors.Count);
        }

        [TestMethod]
        public void ShouldInitailizeWithoutProblems()
        {
            friend.IsDeveloper = true;
            friend.FavoriteLanguageId = 1;
            var wrapper = new FriendWrapper(friend);
            Assert.IsTrue(wrapper.IsValid);
        }
    }
}
