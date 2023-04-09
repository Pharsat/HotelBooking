namespace HotelBooking.Domain
{
    public class Booking
    {
        public long Id { get; set; }

        public byte RoomId { get; set; }

        public long GuestId { get; set; }

        public DateTime ReservationStartDateUtc { get; set; }

        public DateTime ReservationEndDateUtc { get; set; }

        public DateTimeOffset BookingCreationTime { get; set; }

        public DateTimeOffset? BookingModificationTime { get; set; }
    }
}