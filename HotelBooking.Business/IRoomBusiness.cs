namespace HotelBooking.Business
{
    using HotelBooking.Domain;

    public interface IRoomBusiness
    {
        Task<IEnumerable<Room>> GetAllAsync();
    }
}
