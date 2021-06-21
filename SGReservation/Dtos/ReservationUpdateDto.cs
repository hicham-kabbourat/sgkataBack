using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGReservation.Dtos
{
    public class ReservationUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Room { get; set; } // Room 0 to 10
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public DateTime Date { get; set; }
    }
}
