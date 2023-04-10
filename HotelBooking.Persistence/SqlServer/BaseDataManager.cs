namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;

    public class BaseDataManager<TIdT> : IBaseDataManager<TIdT>
    {
        private readonly ISqlConnection _connection;

        public BaseDataManager(ISqlConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByIdAsync(TIdT id, string entityName)
        {
            string query = "SELECT COUNT(*) " +
                           $"FROM [dbo].[{entityName}] " +
                           "WHERE Id = @Id";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync().ConfigureAwait(false);

            var count = (int)(await command.ExecuteScalarAsync().ConfigureAwait(false) ?? 0);

            return count > 0;
        }

        /// <inheritdoc/>
        public async Task RemoveById(TIdT id, string entityName)
        {
            string query = $"DELETE COUNT(*) FROM [dbo].[{entityName}] " +
                           "WHERE Id = @Id";

            using var connection = _connection;

            var command = connection.CreateCommand(query);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync().ConfigureAwait(false);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
    }
}
