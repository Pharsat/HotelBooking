namespace HotelBooking.Persistence.SqlServer
{
    using System.Data.SqlClient;

    public class SqlConnectionWrapper : ISqlConnection
    {
        private readonly SqlConnection _connection;

        public SqlConnectionWrapper(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public async Task OpenAsync()
        {
            await _connection.OpenAsync().ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            await _connection.CloseAsync().ConfigureAwait(false);
        }

        public SqlCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }
    }
}
