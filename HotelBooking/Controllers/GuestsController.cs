namespace HotelBooking.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using HotelBooking.Business;
    using HotelBooking.Domain.Models;
    using HotelBooking.Filter;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    [ExceptionFilter]
    public class GuestsController : ControllerBase
    {
        private readonly IBookingBusiness _bookingBusiness;

        public GuestsController(IBookingBusiness bookingBusiness)
        {
            _bookingBusiness = bookingBusiness;
        }

        [HttpGet]
        [Route("{guestEmail}/bookings")]
        public async Task<IEnumerable<BookingModel>> GetAllForGuest(
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
