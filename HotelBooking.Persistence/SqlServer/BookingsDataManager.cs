namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Models;

    public class BookingsDataManager : BaseDataManager<long>, IBookingsDataManager
    {
        private readonly ISqlConnection _connection;

        public BookingsDataManager(ISqlConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<long> SaveBookingAsync(Booking booking)
        {
            const string query = "INSERT INTO [dbo].[Bookings] ([RoomId], [GuestId], [ReservationStartDateUtc], [ReservationEndDateUtc], [BookingCreationTime]) " +
                                 "VALUES (@RoomId, @GuestId, @ReservationStartDateUtc, @ReservationEndDateUtc, @BookingCreationTime); " +
                                 "SELECT SCOPE_IDENTITY();";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@RoomId", booking.RoomId);
            command.Parameters.AddWithValue("@GuestId", booking.GuestId);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", booking.ReservationStartDateUtc);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", booking.ReservationEndDateUtc);
            command.Parameters.AddWithValue("@BookingCreationTime", booking.BookingCreationTime);

            await connection.OpenAsync().ConfigureAwait(false);

            var idValue = (long)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            return idValue;
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
