namespace HotelBooking.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using HotelBooking.Business;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Models;
    using HotelBooking.Filter;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    [ExceptionFilter]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomBusiness _roomBusiness;
        private readonly IBookingBusiness _bookingBusiness;

        public RoomsController(
            IRoomBusiness roomBusiness,
            IBookingBusiness bookingBusiness)
        {
            _roomBusiness = roomBusiness;
            _bookingBusiness = bookingBusiness;
        }

        [HttpGet]
        public async Task<IEnumerable<Room>> GetAll()
        {
            return await _roomBusiness.GetAllAsync().ConfigureAwait(false);
        }


        [HttpGet]
        [Route("{roomId}/bookings")]
        public async Task<IEnumerable<BookingModel>> GetAll(
            [FromRoute]
            [RegularExpression("^[0-9]*$")]
            byte roomId)
        {
            return await _bookingBusiness
                .GetUpcomingBookingsByRoomAsync(roomId)
                .ConfigureAwait(false);
        }
    }
}
