using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGReservation.Dtos
{
    public class ReservationCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Room { get; set; } // Room 0 to 10
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public DateTime Date { get; set; }
    }
}
