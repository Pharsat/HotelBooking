namespace HotelBooking.Business
{
    using System;

    public interface IBookingValidations
    {
        bool BookedTimeIsLessOrEqualThan3Days(DateTime reservationStartDate, DateTime reservationEndDate);

        bool TheReservationStartDateIsAtLeastNextDay(DateTime reservationStartDate);

        bool ReservationEndDateIsGreaterOrEqualThanReservationStartDate(DateTime reservationStartDate, DateTime reservationEndDate);

        bool BookingIsMade30DaysInAdvance(DateTime reservationStartDate);
    }
}
