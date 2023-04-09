namespace HotelBooking.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateBookingModel
    {
        public UpdateBookingModel()
        {
            GuestEmail = string.Empty;
        }

        [Required]
        public long BookingId { get; set; }

        [Required]
        public byte RoomId { get; set; }

        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; }

        [Required]
        public DateTime ReservationStartDateUtc { get; set; }

        [Required]
        public DateTime ReservationEndDateUtc { get; set; }
    }
}
