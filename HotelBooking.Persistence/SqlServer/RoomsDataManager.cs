namespace HotelBooking.Persistence.SqlServer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;

    public class RoomsDataManager : BaseDataManager<byte>, IRoomsDataManager
    {
        private readonly ISqlConnection _connection;

        public RoomsDataManager(ISqlConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<IList<Room>> GetRoomsAsync()
        {
            var rooms = new List<Room>();

            using var connection = _connection;

            const string query = "SELECT [Id], [Name] FROM [dbo].[Rooms]";

            var command = connection.CreateCommand();
            command.CommandText = query;

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var room = new Room(reader["Name"].ToString()!)
                {
                    Id = (byte)reader["Id"]
                };

                rooms.Add(room);
            }

            return rooms;
        }
    }
}
