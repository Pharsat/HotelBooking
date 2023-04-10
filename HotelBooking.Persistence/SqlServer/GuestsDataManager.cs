namespace HotelBooking.Persistence.SqlServer
{
    using System.Data.Common;
    using HotelBooking.Domain;

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
        public Task<Guest> GetGuestByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<Guest> GetGuestByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}