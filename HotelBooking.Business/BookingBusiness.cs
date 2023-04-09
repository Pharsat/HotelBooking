namespace HotelBooking.Business
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Exceptions;
    using HotelBooking.Persistence;

    public class BookingBusiness : IBookingBusiness
    {
        private readonly IBookingsDataManager _bookingDataManager;
        private readonly IDateTimeUtcProvider _dateTimeProvider;
        private readonly IRoomsDataManager _roomsDataManager;
        private readonly IGuestsDataManager _guestsDataManager;

        public BookingBusiness(
            IBookingsDataManager bookingDataManager,
            IDateTimeUtcProvider dateTimeProvider,
            IRoomsDataManager roomsDataManager,
            IGuestsDataManager guestsDataManager)
        {
            _bookingDataManager = bookingDataManager;
            _dateTimeProvider = dateTimeProvider;
            _roomsDataManager = roomsDataManager;
            _guestsDataManager = guestsDataManager;
        }

        /// <inheritdoc/>
        public async Task BookARoomAsync(byte roomId, string guestEmail, DateTime reservationStartDate, DateTime reservationEndDate)
        {
            MakeCommonValidationsForBooking(reservationStartDate, reservationEndDate);

            using var transactionScope = new TransactionScope();

            if (!await _roomsDataManager.ExistsByIdAsync(roomId).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The room does not exists.");
            }

            long guestId;

            if (!await _guestsDataManager.ExistsByEmailAsync(guestEmail).ConfigureAwait(false))
            {
                guestId = await _guestsDataManager.SaveGuestAsync(guestEmail).ConfigureAwait(false);
            }
            else
            {
                var guest = await _guestsDataManager.GetGuestByEmailAsync(guestEmail).ConfigureAwait(false);
                guestId = guest.Id;
            }

            if (await _bookingDataManager.IsRoomAvailableForBookingAsync(roomId, reservationStartDate, reservationEndDate).ConfigureAwait(false))
            {
                var booking = new Booking
                {
                    RoomId = roomId,
                    GuestId = guestId,
                    ReservationStartDateUtc = reservationStartDate,
                    ReservationEndDateUtc = reservationEndDate,
                    BookingCreationTime = _dateTimeProvider.GetUtcDateTime()
                };

                await _bookingDataManager.SaveBookingAsync(booking).ConfigureAwait(false);
            }
            else
            {
                throw new BookingBusinessException($"The room is not available between the dates  {reservationStartDate} - {reservationEndDate} ");
            }

            transactionScope.Complete();
        }

        /// <inheritdoc/>
        public async Task AlterABookingAsync(long bookingId, byte roomId, string guestEmail, DateTime reservationStartDate, DateTime reservationEndDate)
        {
            MakeCommonValidationsForBooking(reservationStartDate, reservationEndDate);

            using var transactionScope = new TransactionScope();

            if (!await _roomsDataManager.ExistsByIdAsync(roomId).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The room does not exists.");
            }

            if (!await _bookingDataManager.ExistsByIdAsync(bookingId).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The booking does not exists.");
            }

            if (await _bookingDataManager.IsRoomAvailableForBookingAsync(bookingId, roomId, reservationStartDate, reservationEndDate).ConfigureAwait(false))
            {
                var booking = await _bookingDataManager.GetByIdAsync(bookingId).ConfigureAwait(false);

                booking.RoomId = roomId;
                booking.ReservationStartDateUtc = reservationStartDate;
                booking.ReservationEndDateUtc = reservationEndDate;
                booking.BookingModificationTime = _dateTimeProvider.GetUtcDateTime();

                await _bookingDataManager.UpdateABookingAsync(booking).ConfigureAwait(false);
            }
            else
            {
                throw new BookingBusinessException($"The room is not available between the dates  {reservationStartDate} - {reservationEndDate} ");
            }

            transactionScope.Complete();
        }

        /// <inheritdoc/>
        public async Task CancelABookingAsync(long bookingId, string guestEmail)
        {
            using var transactionScope = new TransactionScope();

            if (!await _bookingDataManager.ExistsByIdAsync(bookingId).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The booking does not exists.");
            }

            var booking = await _bookingDataManager.GetByIdAsync(bookingId).ConfigureAwait(false);
            var guest = await _guestsDataManager.GetGuestByIdAsync(booking.GuestId).ConfigureAwait(false);

            if (!guest.Email.Equals(guestEmail, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new BookingBusinessException("The guest does not owns the reservation.");
            }

            await _bookingDataManager.RemoveById(bookingId).ConfigureAwait(false);

            transactionScope.Complete();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Booking>> GetMyUpcomingBookingsForAGuestAsync(string guestEmail)
        {
            return await _bookingDataManager.GetUpcomingBookingsForAGuestAsync(guestEmail, _dateTimeProvider.GetUtcDateTime(), 10).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Booking>> GetUpcomingBookingsByRoomAsync(byte roomId)
        {
            return await _bookingDataManager.GetUpcomingBookingsForARoomAsync(roomId, _dateTimeProvider.GetUtcDateTime(), 10).ConfigureAwait(false);
        }

        /// <summary>
        /// Reusable common validations to perform reservations
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <param name="reservationEndDate">The reservation end date.</param>
        /// <exception cref="BookingBusinessException">Happens in multiple scenarios, described in messages.</exception>
        private void MakeCommonValidationsForBooking(DateTime reservationStartDate, DateTime reservationEndDate)
        {
            if (!ReservationEndDateIsGreaterOrEqualThanReservationStartDate(reservationStartDate, reservationEndDate))
            {
                throw new BookingBusinessException($"The booking from date {reservationStartDate} is greater than booking to date {reservationEndDate}.");
            }

            if (!BookedTimeIsLessOrEqualThan3Days(reservationStartDate, reservationEndDate))
            {
                throw new BookingBusinessException($"The booking time is greater than 3 days {reservationStartDate} - {reservationEndDate}.");
            }

            if (!TheReservationStartDateIsAtLeastNextDay(reservationStartDate))
            {
                throw new BookingBusinessException($"The reservation start date {reservationStartDate} must be at least next day.");
            }

            if (!BookingIsMade30DaysInAdvance(reservationStartDate))
            {
                throw new BookingBusinessException($"The booking is not made 30 days in advance {reservationStartDate}.");
            }
        }

        /// <summary>
        /// Validates the reservation time is not more than 3 days.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <param name="reservationEndDate">The reservation end date.</param>
        /// <returns>The result of the validation.</returns>
        private bool BookedTimeIsLessOrEqualThan3Days(DateTime reservationStartDate, DateTime reservationEndDate)
        {
            return (reservationEndDate - reservationStartDate).Days <= 3;
        }

        /// <summary>
        /// Validates the reserved start date is not today or before.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <returns>The result of the validation.</returns>
        private bool TheReservationStartDateIsAtLeastNextDay(DateTime reservationStartDate)
        {
            return reservationStartDate.Date > _dateTimeProvider.GetUtcDateTime().Date;
        }

        /// <summary>
        /// Validates the reservation start date is less than end date.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <param name="reservationEndDate">The reservation end date.</param>
        /// <returns>The result of the validation.</returns>
        private bool ReservationEndDateIsGreaterOrEqualThanReservationStartDate(DateTime reservationStartDate, DateTime reservationEndDate)
        {
            return reservationEndDate >= reservationStartDate;
        }

        /// <summary>
        /// Validates the reservation is made 30 days in advance.
        /// </summary>
        /// <param name="reservationStartDate">The reservation start date.</param>
        /// <returns>The result of the validation.</returns>
        private bool BookingIsMade30DaysInAdvance(DateTime reservationStartDate)
        {
            var currentDateTime = _dateTimeProvider.GetUtcDateTime();

            return currentDateTime >= reservationStartDate.AddDays(-30);
        }
    }
}
