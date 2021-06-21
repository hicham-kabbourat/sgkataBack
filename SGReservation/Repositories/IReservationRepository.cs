using System.Collections.Generic;
using System.Linq;
using SGReservation.Entities;
using SGReservation.Models;

namespace SGReservation.Repositories
{
    public interface IReservationRepository
    {
        ReservationEntity GetSingle(int id);
        void Add(ReservationEntity item);
        void Delete(int id);
        ReservationEntity Update(int id, ReservationEntity item);
        IQueryable<ReservationEntity> GetAll(QueryParameters queryParameters);

        ICollection<ReservationEntity> GetRandomMeal();
        int Count();

        bool Save();
    }
}
