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
    public class ChangeTrackingCollectionFriendsTests
    {
        private List<FriendWrapper> friends;

        [TestInitialize]
        public void Initialize()
        {
            friends = new List<FriendWrapper>
            {
                new FriendWrapper(new Friend {FirstName = "Bob", LastName="Smith", Address = new()}),
                new FriendWrapper(new Friend {FirstName = "Ted", LastName="Jones", Address = new()}),
            };
        }

        [TestMethod]
        public void ShouldTrackAddedItem()
        {
            var friendToAdd = new FriendWrapper(new Friend { FirstName = "Bill", LastName = "Green", Address = new() });

            var c = new ChangeTrackingCollection<FriendWrapper>(friends);

            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Add(friendToAdd);
            Assert.AreEqual(3, c.Count);
            Assert.AreEqual(1, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(friendToAdd, c.AddedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Remove(friendToAdd);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }

        [TestMethod]
        public void ShouldTrackRemovedItems()
        {
            var friendToRemove = friends.First();
            var c = new ChangeTrackingCollection<FriendWrapper>(friends);
            Assert.AreEqual(2, c.Count);
            Assert.IsFalse(c.IsChanged);

            c.Remove(friendToRemove);
            Assert.AreEqual(1, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(1, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.AreEqual(friendToRemove, c.RemovedItems.First());
            Assert.IsTrue(c.IsChanged);

            c.Add(friendToRemove);
            Assert.AreEqual(2, c.Count);
            Assert.AreEqual(0, c.AddedItems.Count);
            Assert.AreEqual(0, c.RemovedItems.Count);
            Assert.AreEqual(0, c.ModifiedItems.Count);
            Assert.IsFalse(c.IsChanged);
        }
    }
}
