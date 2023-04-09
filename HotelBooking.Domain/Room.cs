namespace HotelBooking.Domain
{
    public class Room
    {
        public Room(string name)
        {
            Name = name;
        }

        public byte Id { get; set; }

        public string Name { get; set; }
    }
}
