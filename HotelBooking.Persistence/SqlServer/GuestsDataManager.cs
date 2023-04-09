namespace HotelBooking.Persistence.SqlServer
{
    using HotelBooking.Domain;

    public class GuestsDataManager : BaseDataManager<long>, IGuestsDataManager
    {
        /// <inheritdoc/>
        public Task<int> SaveGuestAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> ExistsByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<Guest> GetGuestByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<Guest> GetGuestByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}