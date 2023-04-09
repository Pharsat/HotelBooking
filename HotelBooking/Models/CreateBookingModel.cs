namespace HotelBooking.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateBookingModel
    {
        public CreateBookingModel()
        {
            GuestEmail = string.Empty;
        }

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
