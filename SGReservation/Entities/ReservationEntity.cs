using System;

namespace SGReservation.Entities
{
    public class ReservationEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StartHour { get; set; } // 00 to 23
        public string EndHour { get; set; } // 01 to 00
        public DateTime Date { get; set; }
        
        // Foreign Keys
        public int RoomId { get; set; }
        public string Room { get; set; } // Room 0 to 10
    }
}
