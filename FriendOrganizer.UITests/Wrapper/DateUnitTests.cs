using FriendOrganizer.Domain.Models;
using FriendOrganizer.UI.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FriendOrganizer.UITests.Wrapper
{
    [TestClass]
    public class DateUnitTests
    {
        private Meeting meeting;

        [TestInitialize]
        public void Initialize()
        {
            meeting = new Meeting
            {
                Title = "Test",
                DateFrom = new DateTimeOffset(DateTime.Now.AddHours(24)),
                DateTo = new DateTimeOffset(DateTime.Now.AddHours(25)),
            };
        }

        [TestMethod]
        public void ShouldGetValueOfUnderlyingModelProperty()
        {
            var wrapper = new MeetingWrapper(meeting);
            Assert.AreEqual(wrapper.Model.DateFrom, wrapper.DateFrom);
        }



        [TestMethod]
        public void ShouldSetValueOfUnderlyingModelProperty()
        {
            var wrapper = new MeetingWrapper(meeting);
            Assert.AreEqual(wrapper.DateFrom, wrapper.Model.DateFrom );

            var originalDateFrom = wrapper.Model.DateFrom;
            wrapper.DateFrom = originalDateFrom.AddHours(-1);
            Assert.AreNotEqual(originalDateFrom, wrapper.Model.DateFrom);
            Assert.AreEqual(originalDateFrom.AddHours(-1), wrapper.Model.DateFrom);
        }

        [TestMethod]
        public void ShouldRaiseDatePropertyChangedEventOnPropertyChange()
        {
            bool fired = false;
            var wrapper = new MeetingWrapper(meeting);
            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.DateFrom))
                {
                    fired = true;
                }
            };
            // Change the value of DateFrom.
            wrapper.DateFrom = wrapper.DateFrom.AddHours(-1);
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldRaiseMeetingPropertyChangedEventOnPropertyChange()
        {
            bool fired = false;
            var wrapper = new MeetingWrapper(meeting);
            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.Title))
                {
                    fired = true;
                }
            };
            // Change the value of DateFrom.
            wrapper.Title="Changed";
            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldNotRaiseDatePropertyChangedEventOnPropertyNoChange()
        {
            bool fired = false;
            var wrapper = new MeetingWrapper(meeting);
            wrapper.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(wrapper.DateFrom))
                {
                    fired = true;
                }
            };
            // Change the value of DateFrom.
            wrapper.DateFrom = wrapper.DateFrom.AddHours(0);
            Assert.IsFalse(fired);
        }
    }
}

