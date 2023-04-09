namespace HotelBooking.Business
{
    using HotelBooking.Domain.Models;

    public interface IBookingBusiness
    {
        /// <summary>
        /// Books a room.
        /// </summary>
        /// <param name="roomId">The room to book.</param>
        /// <param name="guestEmail">The guest who is booking.</param>
        /// <param name="reservationStartDate">The utc datetime for the booking.</param>
        /// <param name="reservationEndDate">The utc datetime  for the booking.</param>
        /// <returns> an instance of <see cref="Task"/></returns>
        Task BookARoomAsync(byte roomId, string guestEmail, DateTime reservationStartDate, DateTime reservationEndDate);

        /// <summary>
        /// Alters a booking.
        /// </summary>
        /// <param name="bookingId">The booking id.</param>
        /// <param name="roomId">The room to book.</param>
        /// <param name="guestEmail">The guest who is booking.</param>
        /// <param name="reservationStartDate">The utc datetime for the booking.</param>
        /// <param name="reservationEndDate">The utc datetime  for the booking.</param>
        /// <returns> an instance of <see cref="Task"/></returns>
        Task AlterABookingAsync(long bookingId, byte roomId, string guestEmail, DateTime reservationStartDate, DateTime reservationEndDate);

        /// <summary>
        /// Cancels a booking.
        /// </summary>
        /// <param name="bookingId">The booking id.</param>
        /// <param name="guestEmail">The guest email, used to validate the ownership of the booking.</param>
        Task CancelABookingAsync(long bookingId, string guestEmail);

        /// <summary>
        /// Gets all upcoming bookings for a guest.
        /// </summary>
        /// <param name="guestEmail">The guest email.</param>
        /// <returns>A list of bookings.</returns>
        Task<IEnumerable<BookingModel>> GetMyUpcomingBookingsForAGuestAsync(string guestEmail);

        /// <summary>
        /// Gets all upcoming bookings for a room.
        /// </summary>
        /// <param name="roomId">The room id.</param>
        /// <returns>A list of bookings.</returns>
        Task<IEnumerable<BookingModel>> GetUpcomingBookingsByRoomAsync(byte roomId);
    }
}