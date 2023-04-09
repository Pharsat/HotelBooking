namespace HotelBooking.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using HotelBooking.Business;
    using HotelBooking.Domain;
    using HotelBooking.Domain.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
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
        [ResponseCache(Duration = 3600)]
        public async Task<IEnumerable<Room>> GetAll()
        {
            return await _roomBusiness.GetAllAsync().ConfigureAwait(false);
        }


        [HttpGet]
        [Route("{roomId}/bookings")]
        [ResponseCache(Duration = 10)]
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
