namespace HotelBooking.Persistence
{
    using HotelBooking.Domain;

    public interface IGuestsDataManager : IBaseDataManager<long>
    {
        /// <summary>
        /// Saves a guest.
        /// </summary>
        /// <param name="email">The guest email.</param>
        /// <returns>The created guest Id.</returns>
        Task<int> SaveGuestAsync(string email);

        /// <summary>
        /// Verifies if a guest exists by its email.
        /// </summary>
        /// <param name="email">The guest email.</param>
        /// <returns>The guest entity.</returns>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// Get the gets by its email.
        /// </summary>
        /// <param name="email">The guest email.</param>
        /// <returns>The guest entity.</returns>
        Task<Guest> GetGuestByEmailAsync(string email);

        /// <summary>
        /// Get the gets by its id.
        /// </summary>
        /// <param name="id">The guest id.</param>
        /// <returns>The guest entity.</returns>
        Task<Guest> GetGuestByIdAsync(long id);
    }
}
