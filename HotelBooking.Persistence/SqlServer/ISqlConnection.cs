namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Data.SqlClient;

    public interface ISqlConnection : IDisposable
    {
        Task OpenAsync();

        Task CloseAsync();

        SqlCommand CreateCommand(string query);
    }
}
