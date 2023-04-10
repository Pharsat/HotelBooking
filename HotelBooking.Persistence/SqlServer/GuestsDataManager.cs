namespace HotelBooking.Persistence.SqlServer
{
    using HotelBooking.Domain;
    using HotelBooking.Domain.Exceptions;

    public class GuestsDataManager : BaseDataManager<long>, IGuestsDataManager
    {
        private readonly ISqlConnection _connection;

        public GuestsDataManager(ISqlConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public Task<int> SaveGuestAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var rooms = new List<Room>();

            const string query = "SELECT COUNT(*) FROM [dbo].[Guests] WHERE Email = @GuestEmail";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@GuestEmail", email);

            await connection.OpenAsync().ConfigureAwait(false);

            var count = (int)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            return count > 0;
        }

        /// <inheritdoc/>
        public async Task<Guest> GetGuestByEmailAsync(string email)
        {
            const string query = "SELECT [Id], [Email] FROM [dbo].[Guests] WHERE Email = @GuestEmail";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@GuestEmail", email);

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                return new Guest(reader.GetString(reader.GetOrdinal("Email")))
                {
                    Id = reader.GetByte(reader.GetOrdinal("Id"))
                };
            }

            throw new EntityDoesNotExistException($"Guest with email {email} does not exist in our records.");
        }

        /// <inheritdoc/>
        public async Task<Guest> GetGuestByIdAsync(long id)
        {
            const string query = "SELECT [Id], [Email] FROM [dbo].[Guests] WHERE Id = @GuestId";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@GuestId", id);

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                return new Guest(reader.GetString(reader.GetOrdinal("Email")))
                {
                    Id = reader.GetByte(reader.GetOrdinal("Id"))
                };
            }

            throw new EntityDoesNotExistException($"Guest with id {id} does not exist in our records.");
        }
    }
}