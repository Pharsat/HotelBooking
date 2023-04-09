namespace HotelBooking.Business
{
    using System;

    public class DateTimeUtcProvider : IDateTimeUtcProvider
    {
        public DateTime GetUtcDateTime() => DateTime.UtcNow;
    }
}
