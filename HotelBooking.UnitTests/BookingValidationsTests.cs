using HotelBooking.Business;
using Moq;

namespace HotelBooking.UnitTests
{
    [TestClass]
    public class BookingValidationsTests
    {
        [TestInitialize]
        public void TestInit()
        {
            // Your initialization code here
        }

        [TestMethod]
        public void BookedTimeIsLessOrEqualThan3Days_TestFalse()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var reservationEndDate = new DateTime(2023, 1, 4, 23, 59, 59);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 12, 31, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.BookedTimeIsLessOrEqualThan3Days(reservationStartDate, reservationEndDate);

            //Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void BookedTimeIsLessOrEqualThan3Days_TestTrue()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var reservationEndDate = new DateTime(2023, 1, 3, 23, 59, 59);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 12, 31, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.BookedTimeIsLessOrEqualThan3Days(reservationStartDate, reservationEndDate);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TheReservationStartDateIsAtLeastNextDay_TestFalse()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2023, 1, 1, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.TheReservationStartDateIsAtLeastNextDay(reservationStartDate);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TheReservationStartDateIsAtLeastNextDay_TestTrue()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 12, 31, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.TheReservationStartDateIsAtLeastNextDay(reservationStartDate);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReservationEndDateIsGreaterOrEqualThanReservationStartDate_TestTrue()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var reservationEndDate = new DateTime(2023, 1, 3, 23, 59, 59);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 12, 31, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.ReservationEndDateIsGreaterOrEqualThanReservationStartDate(reservationStartDate, reservationEndDate);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReservationEndDateIsGreaterOrEqualThanReservationStartDate_TestFalse()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 3, 23, 59, 59);
            var reservationEndDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 12, 31, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.ReservationEndDateIsGreaterOrEqualThanReservationStartDate(reservationStartDate, reservationEndDate);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BookingIsMade30DaysInAdvance_TestTrue()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var reservationEndDate = new DateTime(2023, 1, 3, 23, 59, 59);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 12, 31, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.BookingIsMade30DaysInAdvance(reservationStartDate);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BookingIsMade30DaysInAdvance_TestFalse()
        {
            //Arrange
            var reservationStartDate = new DateTime(2023, 1, 1, 0, 0, 0);
            var datetimeProvider = new Mock<IDateTimeUtcProvider>();

            datetimeProvider.Setup(mock => mock.GetUtcDateTime()).Returns(new DateTime(2022, 11, 30, 18, 0, 0));

            var bookingValidations = new BookingValidations(datetimeProvider.Object);

            //Act
            var result = bookingValidations.BookingIsMade30DaysInAdvance(reservationStartDate);

            //Assert
            Assert.IsFalse(result);
        }
    }
}