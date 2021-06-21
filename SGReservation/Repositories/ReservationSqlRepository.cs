using SGReservation.Entities;
using SGReservation.Helpers;
using SGReservation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace SGReservation.Repositories
{
    public class ReservationSqlRepository : IReservationRepository
    {
        private readonly ReservationDbContext _reservationDbContext;

        public ReservationSqlRepository(ReservationDbContext reservationDbContext)
        {
            _reservationDbContext = reservationDbContext;
        }

        public ReservationEntity GetSingle(int id)
        {
            return _reservationDbContext.ReservationItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(ReservationEntity item)
        {
            _reservationDbContext.ReservationItems.Add(item);
        }

        public void Delete(int id)
        {
            ReservationEntity reservationItem = GetSingle(id);
            _reservationDbContext.ReservationItems.Remove(reservationItem);
        }

        public ReservationEntity Update(int id, ReservationEntity item)
        {
            _reservationDbContext.ReservationItems.Update(item);
            return item;
        }

        public IQueryable<ReservationEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<ReservationEntity> allItems = _reservationDbContext.ReservationItems.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                allItems = allItems
                    .Where(x => x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _reservationDbContext.ReservationItems.Count();
        }

        public bool Save()
        {
            return (_reservationDbContext.SaveChanges() >= 0);
        }

        public ICollection<ReservationEntity> GetRandomMeal()
        {
            List<ReservationEntity> toReturn = new List<ReservationEntity>();

            toReturn.Add(GetRandomItem("Reservation for dinner"));
            toReturn.Add(GetRandomItem("Reservation"));
            toReturn.Add(GetRandomItem("Business"));

            return toReturn;
        }

        private ReservationEntity GetRandomItem(string name)
        {
            return _reservationDbContext.ReservationItems
                .Where(x => x.Name == name)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
