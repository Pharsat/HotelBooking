namespace HotelBooking.Persistence.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;

    public class RoomsDataManager : BaseDataManager<byte>, IRoomsDataManager
    {
        /// <inheritdoc/>
        public Task<IList<Room>> GetRoomsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
