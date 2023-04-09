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
        [RegularExpression("^[0-9]*$")]
        public long BookingId { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$")]
        public byte RoomId { get; set; }

        [Required]
        [EmailAddress]
        public string GuestEmail { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReservationStartDateUtc { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReservationEndDateUtc { get; set; }
    }
}
