using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UITests.Wrapper
{
    [TestClass]
    public class ChangeNotificationCollectionProperty
    {
        private FriendPhoneNumber friendPhoneNumber;
        private Friend friend;

        [TestInitialize]
        public void Initialize()
        {
            friendPhoneNumber = new FriendPhoneNumber { Number = "+12033131354" };
            friend = new Friend
            {
                FirstName = "Bob",
                LastName = "Smith",
                Address = new(),
                PhoneNumbers = new List<FriendPhoneNumber>
                {
                    new FriendPhoneNumber { Number = "+12033131353"},
                    friendPhoneNumber
                }
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionIfPhoneNumberCollectionIsNull()
        {
            try
            {
                friend.PhoneNumbers = null;
                var wrapper = new FriendWrapper(friend);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("PhoneNumbers cannot be null.", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void ShouldInitializePhoneNumbersProperty()
        {
            var wrapper = new FriendWrapper(friend);

            // wrapper initialized properly
            Assert.IsNotNull(wrapper.PhoneNumbers);
            // Proper number of model phone numbers were wrapped.
            Assert.AreEqual(2, wrapper.PhoneNumbers.Count);
            CheckIfPhoneNumbersCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterRemovingPhoneNumber()
        {
            var wrapper = new FriendWrapper(friend);
            var wpn = wrapper.PhoneNumbers.Single(w => w.Model == friendPhoneNumber);
            wrapper.PhoneNumbers.Remove(wpn);
            CheckIfPhoneNumbersCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterClearingPhoneNumbers()
        {
            var wrapper = new FriendWrapper(friend);
            wrapper.PhoneNumbers.Clear();
            CheckIfPhoneNumbersCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterAddingPhoneNumber()
        {
            friend.PhoneNumbers.Remove(friendPhoneNumber);
            var wrapper = new FriendWrapper(friend);
            wrapper.PhoneNumbers.Add(new FriendPhoneNumberWrapper(friendPhoneNumber));
            CheckIfPhoneNumbersCollectionIsInSync(wrapper);
        }

        private void CheckIfPhoneNumbersCollectionIsInSync(FriendWrapper wrapper)
        {
            // All model phone numbers got wrapped.
            Assert.AreEqual(friend.PhoneNumbers.Count, wrapper.PhoneNumbers.Count);
            // Every phone number can be found as the model in the wrapper collection.
            // In other words, every phone number got wrapped.
            Assert.IsTrue(friend.PhoneNumbers.All(pn =>
                wrapper.PhoneNumbers.Any(pnw => pnw.Model == pn)));
        }
    }
}
