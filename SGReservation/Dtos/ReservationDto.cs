using System;

namespace SGReservation.Dtos
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Room { get; set; } // Room 0 to 10
        public string FirstName { get; set; }
        public string LastName{ get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public DateTime Date { get; set; }
    }
}
