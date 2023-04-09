namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Models;

    public class BookingsDataManager : BaseDataManager<long>, IBookingsDataManager
    {
        /// <inheritdoc/>
        public Task SaveBookingAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task UpdateABookingAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRoomAvailableForBookingAsync(byte roomId, DateTime checkFrom, DateTime checkTo)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRoomAvailableForBookingAsync(long bookingId, byte roomId, DateTime checkFrom, DateTime checkTo)
        {
            throw new NotImplementedException();
        }

        public Task<Booking> GetByIdAsync(long bookingId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<BookingModel>> GetUpcomingBookingsForARoomAsync(byte roomId, DateTime currentDateTimeUtc, int top)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<BookingModel>> GetUpcomingBookingsForAGuestAsync(string guestEmail, DateTime currentDateTimeUtc, int top)
        {
            throw new NotImplementedException();
        }
    }
}
