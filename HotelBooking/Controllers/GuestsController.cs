namespace HotelBooking.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using HotelBooking.Business;
    using HotelBooking.Domain.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly IBookingBusiness _bookingBusiness;

        public GuestsController(IBookingBusiness bookingBusiness)
        {
            _bookingBusiness = bookingBusiness;
        }

        [HttpGet]
        [Route("{guestEmail}/bookings")]
        [ResponseCache(Duration = 10)]
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
