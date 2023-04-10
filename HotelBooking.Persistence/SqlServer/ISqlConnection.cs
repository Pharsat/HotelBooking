namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Data.SqlClient;

    public interface ISqlConnection : IDisposable
    {
        string GetConnectionString();

        Task OpenAsync();

        Task CloseAsync();

        SqlCommand CreateCommand();


    }
}
