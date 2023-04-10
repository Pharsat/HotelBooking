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
        public static readonly string DataBaseEntityName = "Bookings";
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

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@RoomId", booking.RoomId);
            command.Parameters.AddWithValue("@GuestId", booking.GuestId);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", booking.ReservationStartDateUtc);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", booking.ReservationEndDateUtc);
            command.Parameters.AddWithValue("@BookingCreationTime", booking.BookingCreationTime);

            await _connection.OpenAsync().ConfigureAwait(false);

            var idValue = Convert.ToInt64(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            await _connection.CloseAsync().ConfigureAwait(false);

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

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@BookingId", booking.Id);
            command.Parameters.AddWithValue("@RoomId", booking.RoomId);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", booking.ReservationStartDateUtc);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", booking.ReservationEndDateUtc);
            command.Parameters.AddWithValue("@BookingModificationTime", booking.BookingModificationTime);

            await _connection.OpenAsync().ConfigureAwait(false);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await _connection.CloseAsync().ConfigureAwait(false);
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

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", checkFrom);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", checkTo);
            command.Parameters.AddWithValue("@RoomId", roomId);

            await _connection.OpenAsync().ConfigureAwait(false);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            await _connection.CloseAsync().ConfigureAwait(false);

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

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@BookingId", bookingId);
            command.Parameters.AddWithValue("@ReservationStartDateUtc", checkFrom);
            command.Parameters.AddWithValue("@ReservationEndDateUtc", checkTo);
            command.Parameters.AddWithValue("@RoomId", roomId);

            await _connection.OpenAsync().ConfigureAwait(false);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            await _connection.CloseAsync().ConfigureAwait(false);

            return count == 0;
        }

        public async Task<Booking> GetByIdAsync(long bookingId)
        {
            const string query = "SELECT [Id],[RoomId],[GuestId],[ReservationStartDateUtc],[ReservationEndDateUtc],[BookingCreationTime],[BookingModificationTime] " +
                                 "FROM [dbo].[Bookings] " +
                                 "WHERE [Id] = @BookingId;";

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@BookingId", bookingId);

            await _connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var booking = new Booking
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    RoomId = reader.GetByte(reader.GetOrdinal("RoomId")),
                    GuestId = reader.GetInt64(reader.GetOrdinal("GuestId")),
                    ReservationStartDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationStartDateUtc")),
                    ReservationEndDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationEndDateUtc")),
                    BookingCreationTime = reader.GetDateTimeOffset(reader.GetOrdinal("BookingCreationTime")),
                    BookingModificationTime = !reader.IsDBNull(reader.GetOrdinal("BookingModificationTime")) ? reader.GetDateTimeOffset(reader.GetOrdinal("BookingModificationTime")) : null
                };

                await _connection.CloseAsync().ConfigureAwait(false);

                return booking;
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

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@RoomId", roomId);
            command.Parameters.AddWithValue("@CurrentDateTimeUtc", currentDateTimeUtc);
            command.Parameters.AddWithValue("@Top", top);

            await _connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            var bookings = new List<BookingModel>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var booking = new BookingModel(reader.GetString(reader.GetOrdinal("RoomName")))
                {
                    BookingId = reader.GetInt64(reader.GetOrdinal("Id")),
                    ReservationStartDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationStartDateUtc")),
                    ReservationEndDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationEndDateUtc"))
                };

                bookings.Add(booking);
            }

            await _connection.CloseAsync().ConfigureAwait(false);

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

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@GuestEmail", guestEmail);
            command.Parameters.AddWithValue("@CurrentDateTimeUtc", currentDateTimeUtc);
            command.Parameters.AddWithValue("@Top", top);

            await _connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            var bookings = new List<BookingModel>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var booking = new BookingModel(reader.GetString(reader.GetOrdinal("RoomName")))
                {
                    BookingId = reader.GetInt64(reader.GetOrdinal("Id")),
                    ReservationStartDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationStartDateUtc")),
                    ReservationEndDateUtc = reader.GetDateTime(reader.GetOrdinal("ReservationEndDateUtc"))
                };

                bookings.Add(booking);
            }

            await _connection.CloseAsync().ConfigureAwait(false);

            return bookings;
        }
    }
}
