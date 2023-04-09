namespace HotelBooking.Persistence
{
    using System.Threading.Tasks;
    using HotelBooking.Domain;

    public interface IBookingsDataManager : IBaseDataManager<long>
    {
        /// <summary>
        /// Stores a booking to persistence.
        /// </summary>
        /// <param name="booking">The booking object to store.</param>
        /// <returns>An Instance of <see cref="Booking"/></returns>
        Task SaveBookingAsync(Booking booking);

        /// <summary>
        /// Modify a booking to persistence.
        /// </summary>
        /// <param name="booking">The booking object to store.</param>
        /// <returns>An Instance of <see cref="Booking"/></returns>
        Task UpdateABookingAsync(Booking booking);

        /// <summary>
        /// Checks if room is available for booking.
        /// </summary>
        /// <param name="roomId">The room id</param>
        /// <param name="checkFrom"></param>
        /// <param name="checkTo"></param>
        /// <returns>A flag whether the room is available or not.</returns>
        Task<bool> IsRoomAvailableForBookingAsync(byte roomId, DateTime checkFrom, DateTime checkTo);

        /// <summary>
        /// Checks if room is available for booking but excludes current book.
        /// </summary>
        /// <param name="bookingId">Current booking id.</param>
        /// <param name="roomId">The room id</param>
        /// <param name="checkFrom"></param>
        /// <param name="checkTo"></param>
        /// <returns>A flag whether the room is available or not.</returns>
        Task<bool> IsRoomAvailableForBookingAsync(long bookingId, byte roomId, DateTime checkFrom, DateTime checkTo);

        /// <summary>
        /// Gets a booking by its Id.
        /// </summary>
        /// <param name="bookingId">The booking Id.</param>
        /// <returns>The booking object.</returns>
        Task<Booking> GetByIdAsync(long bookingId);

        /// <summary>
        /// Gets the upcoming bookings.
        /// </summary>
        /// <param name="roomId">The room id.</param>
        /// <param name="currentDateTimeUtc">The current date time to filter the upcoming reservations.</param>
        /// <param name="top">The limit of reservations.</param>
        /// <returns>The top list of booking after the specified date.</returns>
        Task<IEnumerable<Booking>> GetUpcomingBookingsForARoomAsync(byte roomId, DateTime currentDateTimeUtc, int top);

        /// <summary>
        /// Gets the upcoming bookings for a guest.
        /// </summary>
        /// <param name="guestEmail">Get the guest email.</param>
        /// <param name="currentDateTimeUtc">The current date time to filter the upcoming reservations.</param>
        /// <param name="top">The limit of reservations.</param>
        /// <returns>The top list of booking after the specified date.</returns>
        Task<IEnumerable<Booking>> GetUpcomingBookingsForAGuestAsync(string guestEmail, DateTime currentDateTimeUtc, int top);
    }
}
