namespace HotelBooking.Business
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotelBooking.Domain;
    using HotelBooking.Persistence;

    public class RoomBusiness : IRoomBusiness
    {
        private readonly IRoomsDataManager _roomsDataManager;

        public RoomBusiness(IRoomsDataManager roomsDataManager)
        {
            _roomsDataManager = roomsDataManager;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _roomsDataManager.GetRoomsAsync().ConfigureAwait(false);
        }
    }
}
