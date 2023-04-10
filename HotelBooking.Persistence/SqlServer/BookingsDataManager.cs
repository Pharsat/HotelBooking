namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Exceptions;
    using HotelBooking.Domain.Models;

    public class BookingsDataManager : BaseDataManager<long>, IBookingsDataManager
    {
        private readonly ISqlConnection _connection;

        public BookingsDataManager(ISqlConnection connection) : base(connection)
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
        public async Task UpdateABookingAsync(Booking booking)
        {
            const string query = "UPDATE [dbo].[Bookings] SET " +
                                 "[RoomId] = @RoomId," +
                                 "[ReservationStartDateUtc] = @ReservationStartDateUtc," +
                                 "[ReservationEndDateUtc] = @ReservationEndDateUtc," +
                                 "[BookingModificationTime] = @BookingModificationTime " +
                                 "WHERE [Id] = @BookingId;";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@BookingId", booking.Id);
            command.Parameters.AddWithValue("@RoomId", booking.RoomId);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", booking.ReservationStartDateUtc);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", booking.ReservationEndDateUtc);
            command.Parameters.AddWithValue("@BookingModificationTime", booking.BookingModificationTime);

            await connection.OpenAsync().ConfigureAwait(false);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> IsRoomAvailableForBookingAsync(byte roomId, DateTime checkFrom, DateTime checkTo)
        {
            const string query = "SELECT COUNT(*) " +
                                 "FROM [dbo].[Bookings] " +
                                 "WHERE (" +
                                 "([ReservationStartDateUtc] BETWEEN @ReservationStartDateUtc AND @ReservationStartDateUtc) OR " +
                                 "([ReservationEndDateUtc] BETWEEN @ReservationStartDateUtc AND @ReservationEndDateUtc)" +
                                 ") AND " +
                                 "[RoomId] = @RoomId;";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", checkFrom);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", checkTo);
            command.Parameters.AddWithValue("@RoomId", roomId);

            await connection.OpenAsync().ConfigureAwait(false);

            var count = (int)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            return count == 0;
        }

        /// <inheritdoc/>
        public async Task<bool> IsRoomAvailableForBookingAsync(long bookingId, byte roomId, DateTime checkFrom, DateTime checkTo)
        {
            const string query = "SELECT COUNT(*) " +
                                 "FROM [dbo].[Bookings] " +
                                 "WHERE (" +
                                 "([ReservationStartDateUtc] BETWEEN @ReservationStartDateUtc AND @ReservationStartDateUtc) OR " +
                                 "([ReservationEndDateUtc] BETWEEN @ReservationStartDateUtc AND @ReservationEndDateUtc)" +
                                 ") AND " +
                                 "[RoomId] = @RoomId AND" +
                                 "[Id] != @BookingId";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@BookingId", bookingId);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", checkFrom);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", checkTo);
            command.Parameters.AddWithValue("@RoomId", roomId);

            await connection.OpenAsync().ConfigureAwait(false);

            var count = (int)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            return count == 0;
        }

        public async Task<Booking> GetByIdAsync(long bookingId)
        {
            const string query = "SELECT [Id],[RoomId],[GuestId],[ReservationStartDateUtc],[ReservationEndDateUtc],[BookingCreationTime],[BookingModificationTime] " +
                                 "FROM [dbo].[Bookings] " +
                                 "WHERE [Id] = @BookingId;";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@BookingId", bookingId);

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                return new Booking
                {
                    Id = reader.GetByte(reader.GetOrdinal("Id")),
                    RoomId = reader.GetByte(reader.GetOrdinal("RoomId")),
                    GuestId = reader.GetByte(reader.GetOrdinal("GuestId")),
                    ReservationStartDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationStartDateUtc")),
                    ReservationEndDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationEndDateUtc")),
                    BookingCreationTime = reader.GetDateTimeOffset(reader.GetOrdinal("BookingCreationTime")),
                    BookingModificationTime = reader.GetDateTimeOffset(reader.GetOrdinal("BookingModificationTime"))
                };
            }

            throw new EntityDoesNotExistException($"Booking with Id {bookingId} does not exist in our records.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BookingModel>> GetUpcomingBookingsForARoomAsync(byte roomId, DateTime currentDateTimeUtc, int top)
        {
            const string query = "SELECT TOP (@Top) " +
                                 "[dbo].[Bookings].[Id]," +
                                 "[dbo].[Rooms].[Name] AS [RoomName]," +
                                 "[ReservationStartDateUtc]," +
                                 "[ReservationEndDateUtc] " +
                                 "FROM [dbo].[Bookings] " +
                                 "INNER JOIN [dbo].[Rooms] ON [dbo].[Rooms].Id = [dbo].[Bookings].RoomId " +
                                 "WHERE [RoomId] = @RoomId AND " +
                                 "[ReservationStartDateUtc] >= @CurrentDateTimeUtc;";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@RoomId", roomId);
            command.Parameters.AddWithValue("@CurrentDateTimeUtc", currentDateTimeUtc);
            command.Parameters.AddWithValue("@Top", top);

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            var bookings = new List<BookingModel>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var booking = new BookingModel(reader.GetString(reader.GetOrdinal("RoomName")))
                {
                    BookingId = reader.GetByte(reader.GetOrdinal("Id")),
                    ReservationStartDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationStartDateUtc")),
                    ReservationEndDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationEndDateUtc"))
                };

                bookings.Add(booking);
            }

            return bookings;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BookingModel>> GetUpcomingBookingsForAGuestAsync(string guestEmail, DateTime currentDateTimeUtc, int top)
        {
            const string query = "SELECT TOP (@Top) " +
                                 "[dbo].[Bookings].[Id]," +
                                 "[dbo].[Rooms].[Name] AS [RoomName]," +
                                 "[ReservationStartDateUtc]," +
                                 "[ReservationEndDateUtc] " +
                                 "FROM [dbo].[Bookings] " +
                                 "INNER JOIN [dbo].[Rooms] ON [dbo].[Rooms].Id = [dbo].[Bookings].RoomId " +
                                 "INNER JOIN [dbo].[Guests] ON [dbo].[Guests].Id = [dbo].[Bookings].GuestId " +
                                 "WHERE [dbo].[Guests].[Email] = @GuestEmail AND " +
                                 "[ReservationStartDateUtc] >= @CurrentDateTimeUtc;";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@GuestEmail", guestEmail);
            command.Parameters.AddWithValue("@CurrentDateTimeUtc", currentDateTimeUtc);
            command.Parameters.AddWithValue("@Top", top);

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            var bookings = new List<BookingModel>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var booking = new BookingModel(reader.GetString(reader.GetOrdinal("RoomName")))
                {
                    BookingId = reader.GetByte(reader.GetOrdinal("Id")),
                    ReservationStartDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationStartDateUtc")),
                    ReservationEndDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationEndDateUtc"))
                };

                bookings.Add(booking);
            }

            return bookings;
        }
    }
}
