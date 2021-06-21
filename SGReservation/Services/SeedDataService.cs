using SGReservation.Entities;
using SGReservation.Repositories;
using System;
using System.Threading.Tasks;

namespace SGReservation.Services
{
    public class SeedDataService : ISeedDataService
    {
        public async Task Initialize(ReservationDbContext context)
        {
            context.ReservationItems.Add(new ReservationEntity() { LastName = "Dupont", RoomId = 1, Room ="Room1", Name = "Reservation Dinner", Date = DateTime.Now.Add(TimeSpan.FromDays(-2)), StartHour  = "08", EndHour = "11"});
            context.ReservationItems.Add(new ReservationEntity() { LastName = "Dubois", RoomId = 2, Room = "Room2", Name = "Reservation Break fast", Date = DateTime.Now, StartHour = "08", EndHour = "11" });
            context.ReservationItems.Add(new ReservationEntity() { LastName = "DuJardin", RoomId = 3, Room = "Room3", Name = "Reservation Lunch", Date = DateTime.Now, StartHour = "10", EndHour = "11" });
            context.ReservationItems.Add(new ReservationEntity() { LastName = "DuSwarte", RoomId = 4, Room = "Room4", Name = "Reservation Night", Date = DateTime.Now , StartHour = "21", EndHour = "22" });

            await context.SaveChangesAsync();
        }
    }
}
