﻿namespace HotelBooking.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using HotelBooking.Business;
    using HotelBooking.Domain;
    using HotelBooking.Filter;
    using HotelBooking.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    [ExceptionFilter]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingBusiness _bookingBusiness;

        public BookingsController(IBookingBusiness bookingBusiness)
        {
            _bookingBusiness = bookingBusiness;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingModel createBookingModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _bookingBusiness
                .BookARoomAsync(
                    createBookingModel.RoomId,
                    createBookingModel.GuestEmail,
                    createBookingModel.ReservationStartDateUtc,
                    createBookingModel.ReservationEndDateUtc)
                .ConfigureAwait(false);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateBookingModel updateBookingModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _bookingBusiness
                .AlterABookingAsync(
                    updateBookingModel.BookingId,
                    updateBookingModel.RoomId,
                    updateBookingModel.GuestEmail,
                    updateBookingModel.ReservationStartDateUtc,
                    updateBookingModel.ReservationEndDateUtc)
                .ConfigureAwait(false);

            return Ok();
        }

        [HttpDelete]
        [Route("{bookingId}/{guestEmail}")]
        public async Task<IActionResult> Delete(
            [FromRoute]
            long bookingId,
            [FromRoute]
            [EmailAddress]
            string guestEmail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _bookingBusiness
                .CancelABookingAsync(bookingId, guestEmail)
                .ConfigureAwait(false);

            return Ok();
        }


        [HttpGet]
        [Route("{guestEmail}")]
        public async Task<IEnumerable<Booking>> GetAllForGuest(
            [FromRoute]
            [EmailAddress]
            string guestEmail)
        {
            return await _bookingBusiness
                .GetMyUpcomingBookingsForAGuestAsync(guestEmail)
                .ConfigureAwait(false);
        }
    }
}
