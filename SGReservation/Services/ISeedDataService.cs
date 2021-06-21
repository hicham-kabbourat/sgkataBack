using SGReservation.Repositories;
using System.Threading.Tasks;

namespace SGReservation.Services
{
    public interface ISeedDataService
    {
        Task Initialize(ReservationDbContext context);
    }
}
