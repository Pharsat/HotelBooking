namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;

    public class BaseDataManager<TIdT> : IBaseDataManager<TIdT>
    {
        private readonly ISqlConnection _connection;

        protected BaseDataManager(ISqlConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByIdAsync(TIdT id, string entityName)
        {
            string query = "SELECT COUNT(*) " +
                           $"FROM [dbo].[{entityName}] " +
                           "WHERE Id = @Id";

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@Id", id);

            await _connection.OpenAsync().ConfigureAwait(false);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            await _connection.CloseAsync().ConfigureAwait(false);

            return count > 0;
        }

        /// <inheritdoc/>
        public async Task RemoveById(TIdT id, string entityName)
        {
            string query = $"DELETE FROM [dbo].[{entityName}] " +
                           "WHERE Id = @Id";

            var command = _connection.CreateCommand(query);
            command.Parameters.AddWithValue("@Id", id);

            await _connection.OpenAsync().ConfigureAwait(false);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await _connection.CloseAsync().ConfigureAwait(false);
        }
    }
}
