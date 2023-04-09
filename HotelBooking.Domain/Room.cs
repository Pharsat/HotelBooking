namespace HotelBooking.Domain
{
    public class Room
    {
        public Room(string name)
        {
            Name = name;
        }

        private byte Id { get; set; }

        public string Name { get; set; }
    }
}
