namespace HotelBooking.Domain.Models
{
    using System;

    public class BookingModel
    {
        public BookingModel(string roomName)
        {
            RoomName = roomName;
        }

        public long BookingId { get; set; }

        public string RoomName{ get; set; }

        public DateTime ReservationStartDateUtc { get; set; }

        public DateTime ReservationEndDateUtc { get; set; }
    }
}
