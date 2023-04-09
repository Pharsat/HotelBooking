namespace HotelBooking.Domain
{
    public class Guest
    {
        public Guest(string email)
        {
            Email = email;
        }

        public long Id { get; set; }

        public string Email { get; set; }
    }
}
