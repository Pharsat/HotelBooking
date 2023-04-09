namespace HotelBooking.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;

    public interface IRoomsDataManager : IBaseDataManager<byte>
    {
        /// <summary>
        /// Get all the rooms.
        /// </summary>
        /// <returns>The list of rooms</returns>
        Task<IList<Room>> GetRoomsAsync();
    }
}
