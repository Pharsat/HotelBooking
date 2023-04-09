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
            this._connection = connection;
        }

        /// <inheritdoc/>
        public async Task<IList<Room>> GetRoomsAsync()
        {
            var rooms = new List<Room>();

            try
            {
                await _connection.OpenAsync().ConfigureAwait(false);

                const string query = "SELECT [Id], [Name] FROM [HotelBooker].[dbo].[Rooms]";

                var command = _connection.CreateCommand();
                command.CommandText = query;

                await using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);


                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var room = new Room(reader["Name"].ToString()!)
                    {
                        Id = (byte)reader["Id"]
                    };

                    rooms.Add(room);
                }
            }
            finally
            {
                await _connection.CloseAsync().ConfigureAwait(false);
            }

            return rooms;
        }
    }
}
