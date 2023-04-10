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

            const string query = "SELECT [Id], [Name] FROM [dbo].[Rooms]";

            using var connection = _connection;

            var command = connection.CreateCommand(query);

            await connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var room = new Room(reader.GetString(reader.GetOrdinal("Name")))
                {
                    Id = reader.GetByte(reader.GetOrdinal("Id"))
                };

                rooms.Add(room);
            }

            return rooms;
        }
    }
}
