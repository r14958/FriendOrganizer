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
    public class ChangeTrackingCollectionTests
    {
        private List<FriendPhoneNumberWrapper> phoneNumbers;

        [TestInitialize]
        public void Initialize()
        {
            phoneNumbers = new List<FriendPhoneNumberWrapper>
            {
                new FriendPhoneNumberWrapper(new FriendPhoneNumber { Number="555-5555"}),
                new FriendPhoneNumberWrapper(new FriendPhoneNumber { Number="777-7777"}),
            };
            }

        [TestMethod]
        public void ShouldTrackAddedItem()
        {
            var phoneToAdd = new FriendPhoneNumberWrapper(new FriendPhoneNumber());

            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Add(phoneToAdd);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(phoneToAdd, c.AddedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Remove(phoneToAdd);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackRemovedItems()
        {
            var phoneToRemove = phoneNumbers.First();
            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Remove(phoneToRemove);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(phoneToRemove, c.RemovedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Add(phoneToRemove);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);

        }

        [TestMethod]
        public void ShouldTrackModified()
        {
            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);
            var originalNumber = c.First().Number;
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.First().Number = "222-2222";
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual("222-2222", c.ModifiedItems.First().Number);
            Assert.IsTrue(c.IsChanged);

            c.First().Number = originalNumber;
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(originalNumber, c.First().Number);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldNotTrackAddedAsModified()
        {
            var phoneToAdd = new FriendPhoneNumberWrapper(new FriendPhoneNumber());

            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Add(phoneToAdd);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(phoneToAdd, c.AddedItems.First());
            Assert.IsTrue(c.IsChanged);

            phoneToAdd.Number = "222-2222";
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual("222-2222", c.Last().Number);
            Assert.IsTrue(c.IsChanged);
        }

        [TestMethod]
        public void ShouldNotTrackRemovedItemAsModified()
        {
            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            phoneNumbers.Last().Number = "222-2222";
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.AreEqual("222-2222", c.ModifiedItems.First().Number);
            Assert.IsTrue(c.IsChanged);
            
            var phoneToRemove = phoneNumbers.Last();
            c.Remove(phoneToRemove);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(phoneToRemove, c.RemovedItems.First());
            Assert.IsTrue(c.IsChanged);
        }

        [TestMethod]
        public void ShouldAcceptChanges()
        {
            var phoneToModify = phoneNumbers.First();
            var originalNumber = phoneToModify.Number;
            var phoneToRemove = phoneNumbers.Skip(1).First();
            var phoneToAdd = new FriendPhoneNumberWrapper(new FriendPhoneNumber { Number = "222-2222" });

            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);

            c.Add(phoneToAdd);
            c.Remove(phoneToRemove);
            phoneToModify.Number = "111-1111";
            
            Assert.AreEqual(originalNumber, phoneToModify.NumberOriginalValue);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.IsTrue(c.IsChanged);

            c.AcceptChanges();
            Assert.AreNotEqual(originalNumber, phoneToModify.NumberOriginalValue);
            Assert.AreEqual("111-1111", phoneToModify.NumberOriginalValue);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectChanges()
        {
            var phoneToModify = phoneNumbers.First();
            var originalNumber = phoneToModify.Number;
            var phoneToRemove = phoneNumbers.Skip(1).First();
            var phoneToAdd = new FriendPhoneNumberWrapper(new FriendPhoneNumber { Number = "222-2222" });

            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);

            c.Add(phoneToAdd);
            c.Remove(phoneToRemove);
            phoneToModify.Number = "111-1111";

            Assert.AreEqual(originalNumber, phoneToModify.NumberOriginalValue);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(1, c.ModifiedItems.Count);
            Assert.IsTrue(c.IsChanged);

            c.RejectChanges();
            
            Assert.AreEqual(originalNumber, phoneToModify.NumberOriginalValue);
            Assert.AreEqual("555-5555", phoneToModify.Number);
            Assert.AreEqual("555-5555", phoneToModify.NumberOriginalValue);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldRejectModifiedAndRemovedItem()
        {
            var phoneToModify = phoneNumbers.First();
            var originalNumber = phoneToModify.Number;

            var c = new ChangeTrackingCollection<FriendPhoneNumberWrapper>(phoneNumbers);
            
            phoneToModify.Number = "111-1111";
            c.Remove(phoneToModify);

            c.RejectChanges();

            Assert.AreEqual(originalNumber, phoneToModify.NumberOriginalValue);
            Assert.AreEqual("555-5555", phoneToModify.Number);
            Assert.AreEqual("555-5555", phoneToModify.NumberOriginalValue);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);

        }

    }
}
