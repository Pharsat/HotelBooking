namespace HotelBooking.Business
{
    using System;

    public interface IDateTimeUtcProvider
    {
        DateTime GetUtcDateTime();
    }
}
