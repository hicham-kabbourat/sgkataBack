using Microsoft.EntityFrameworkCore;
using SGReservation.Entities;

namespace SGReservation.Repositories
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options)
           : base(options)
        {

        }

        public DbSet<ReservationEntity> ReservationItems { get; set; }

    }
}
