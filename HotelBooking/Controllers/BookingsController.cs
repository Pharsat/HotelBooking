namespace HotelBooking.Controllers
{
    using HotelBooking.Business;
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
    }
}
