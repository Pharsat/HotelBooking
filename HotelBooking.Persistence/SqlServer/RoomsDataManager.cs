namespace HotelBooking.Persistence.SqlServer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;

    public class RoomsDataManager : BaseDataManager<byte>, IRoomsDataManager
    {
        public static readonly string DataBaseEntityName = "Rooms";
        private readonly ISqlConnection _connection;

        public RoomsDataManager(ISqlConnection connection) : base(connection)
        {
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<IList<Room>> GetRoomsAsync()
        {
            const string query = "SELECT [Id], [Name] " +
                                 "FROM [dbo].[Rooms]";

            var command = _connection.CreateCommand(query);

            await _connection.OpenAsync().ConfigureAwait(false);
            await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            var rooms = new List<Room>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var room = new Room(reader.GetString(reader.GetOrdinal("Name")))
                {
                    Id = reader.GetByte(reader.GetOrdinal("Id"))
                };

                rooms.Add(room);
            }

            await _connection.CloseAsync().ConfigureAwait(false);

            return rooms;
        }
    }
}
