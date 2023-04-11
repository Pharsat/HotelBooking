namespace HotelBooking.Business
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Exceptions;
    using HotelBooking.Domain.Models;
    using HotelBooking.Persistence;
    using HotelBooking.Persistence.SqlServer;

    public class BookingBusiness : IBookingBusiness
    {
        private readonly IBookingsDataManager _bookingDataManager;
        private readonly IDateTimeUtcProvider _dateTimeProvider;
        private readonly IRoomsDataManager _roomsDataManager;
        private readonly IGuestsDataManager _guestsDataManager;
        private readonly IBookingValidations _bookingValidations;

        public BookingBusiness(
            IBookingsDataManager bookingDataManager,
            IDateTimeUtcProvider dateTimeProvider,
            IRoomsDataManager roomsDataManager,
            IGuestsDataManager guestsDataManager,
            IBookingValidations bookingValidations)
        {
            _bookingDataManager = bookingDataManager;
            _dateTimeProvider = dateTimeProvider;
            _roomsDataManager = roomsDataManager;
            _guestsDataManager = guestsDataManager;
            _bookingValidations = bookingValidations;
        }

        /// <inheritdoc/>
        public async Task<Booking> BookARoomAsync(byte roomId, string guestEmail, DateTime reservationStartDate, DateTime reservationEndDate)
        {
            reservationStartDate = new DateTime(reservationStartDate.Year, reservationStartDate.Month, reservationStartDate.Day, 0, 0, 0);
            reservationEndDate = new DateTime(reservationEndDate.Year, reservationEndDate.Month, reservationEndDate.Day, 23, 59, 59);

            MakeCommonValidationsForBooking(reservationStartDate, reservationEndDate);

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (!await _roomsDataManager.ExistsByIdAsync(roomId, RoomsDataManager.DataBaseEntityName).ConfigureAwait(false))
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

                var bookingId = await _bookingDataManager.SaveBookingAsync(booking).ConfigureAwait(false);

                transactionScope.Complete();

                booking.Id = bookingId;

                return booking;
            }
            else
            {
                throw new BookingBusinessException($"The room is not available between the dates  {reservationStartDate} - {reservationEndDate} ");
            }
        }

        /// <inheritdoc/>
        public async Task AlterABookingAsync(long bookingId, byte roomId, string guestEmail, DateTime reservationStartDate, DateTime reservationEndDate)
        {
            reservationStartDate = new DateTime(reservationStartDate.Year, reservationStartDate.Month, reservationStartDate.Day, 0, 0, 0);
            reservationEndDate = new DateTime(reservationEndDate.Year, reservationEndDate.Month, reservationEndDate.Day, 23, 59, 59);

            MakeCommonValidationsForBooking(reservationStartDate, reservationEndDate);

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (!await _roomsDataManager.ExistsByIdAsync(roomId, RoomsDataManager.DataBaseEntityName).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The room does not exists.");
            }

            if (!await _bookingDataManager.ExistsByIdAsync(bookingId, BookingsDataManager.DataBaseEntityName).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The booking does not exists.");
            }

            if (await _bookingDataManager.IsRoomAvailableForBookingAsync(bookingId, roomId, reservationStartDate, reservationEndDate).ConfigureAwait(false))
            {
                var booking = await _bookingDataManager.GetByIdAsync(bookingId).ConfigureAwait(false);
                var guest = await _guestsDataManager.GetGuestByIdAsync(booking.GuestId).ConfigureAwait(false);

                if (!guestEmail.Equals(guest.Email, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new BookingBusinessException($"The given email {guestEmail} does not match the email of the owner of this booking.");
                }

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
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (!await _bookingDataManager.ExistsByIdAsync(bookingId, BookingsDataManager.DataBaseEntityName).ConfigureAwait(false))
            {
                throw new BookingBusinessException("The booking does not exists.");
            }

            var booking = await _bookingDataManager.GetByIdAsync(bookingId).ConfigureAwait(false);
            var guest = await _guestsDataManager.GetGuestByIdAsync(booking.GuestId).ConfigureAwait(false);

            if (!guest.Email.Equals(guestEmail, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new BookingBusinessException("The guest does not owns the reservation.");
            }

            await _bookingDataManager.RemoveById(bookingId, BookingsDataManager.DataBaseEntityName).ConfigureAwait(false);

            transactionScope.Complete();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BookingModel>> GetMyUpcomingBookingsForAGuestAsync(string guestEmail)
        {
            return await _bookingDataManager.GetUpcomingBookingsForAGuestAsync(guestEmail, _dateTimeProvider.GetUtcDateTime(), 10).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BookingModel>> GetUpcomingBookingsByRoomAsync(byte roomId)
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
            if (!_bookingValidations.ReservationEndDateIsGreaterOrEqualThanReservationStartDate(reservationStartDate, reservationEndDate))
            {
                throw new BookingBusinessException($"The booking from date {reservationStartDate} is greater than booking to date {reservationEndDate}.");
            }

            if (!_bookingValidations.BookedTimeIsLessOrEqualThan3Days(reservationStartDate, reservationEndDate))
            {
                throw new BookingBusinessException($"The booking time is greater than 3 days {reservationStartDate} - {reservationEndDate}.");
            }

            if (!_bookingValidations.TheReservationStartDateIsAtLeastNextDay(reservationStartDate))
            {
                throw new BookingBusinessException($"The reservation start date {reservationStartDate} must be at least next day.");
            }

            if (!_bookingValidations.BookingIsMade30DaysInAdvance(reservationStartDate))
            {
                throw new BookingBusinessException($"The booking is not made 30 days in advance {reservationStartDate}.");
            }
        }
    }
}
