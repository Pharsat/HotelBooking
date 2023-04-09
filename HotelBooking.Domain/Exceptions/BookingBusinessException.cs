namespace HotelBooking.Domain.Exceptions
{
    using System;

    public class BookingBusinessException : Exception
    {
        public BookingBusinessException()
        {
        }

        public BookingBusinessException(string message) : base(message)
        {
        }

        public BookingBusinessException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
