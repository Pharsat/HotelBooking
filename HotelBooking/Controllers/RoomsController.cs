namespace HotelBooking.Controllers
{
    using HotelBooking.Business;
    using HotelBooking.Domain;
    using HotelBooking.Filter;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    [ExceptionFilter]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomBusiness _roomBusiness;

        public RoomsController(IRoomBusiness roomBusiness)
        {
            _roomBusiness = roomBusiness;
        }

        [HttpGet]
        public async Task<IEnumerable<Room>> GetAll()
        {
            return await _roomBusiness.GetAllAsync().ConfigureAwait(false);
        }
    }
}
