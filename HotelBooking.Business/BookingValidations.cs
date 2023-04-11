namespace HotelBooking.Business
{
    using System;

    public class BookingValidations : IBookingValidations
    {
        private readonly IDateTimeUtcProvider _dateTimeProvider;

        public BookingValidations(IDateTimeUtcProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Validates the reservation time is not more than 3 days.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <param name="reservationEndDate">The reservation end date.</param>
        /// <returns>The result of the validation.</returns>
        public bool BookedTimeIsLessOrEqualThan3Days(DateTime reservationStartDate, DateTime reservationEndDate)
        {
            return ((reservationEndDate.Date - reservationStartDate.Date).Days + 1) <= 3;
        }

        /// <summary>
        /// Validates the reserved start date is not today or before.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <returns>The result of the validation.</returns>
        public bool TheReservationStartDateIsAtLeastNextDay(DateTime reservationStartDate)
        {
            return reservationStartDate.Date > _dateTimeProvider.GetUtcDateTime().Date;
        }

        /// <summary>
        /// Validates the reservation start date is less than end date.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <param name="reservationEndDate">The reservation end date.</param>
        /// <returns>The result of the validation.</returns>
        public bool ReservationEndDateIsGreaterOrEqualThanReservationStartDate(DateTime reservationStartDate, DateTime reservationEndDate)
        {
            return reservationEndDate >= reservationStartDate;
        }

        /// <summary>
        /// Validates the reservation is made 30 days in advance.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <returns>The result of the validation.</returns>
        public bool BookingIsMade30DaysInAdvance(DateTime reservationStartDate)
        {
            var currentDateTime = _dateTimeProvider.GetUtcDateTime();

            return currentDateTime >= reservationStartDate.AddDays(-30);
        }
    }
}
