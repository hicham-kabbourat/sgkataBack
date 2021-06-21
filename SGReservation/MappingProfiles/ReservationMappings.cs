using AutoMapper;
using SGReservation.Dtos;
using SGReservation.Entities;

namespace SGReservation.MappingProfiles
{
    public class ReservationMappings : Profile
    {
        public ReservationMappings()
        {
            CreateMap<ReservationEntity, ReservationDto>().ReverseMap();
            CreateMap<ReservationEntity, ReservationUpdateDto>().ReverseMap();
            CreateMap<ReservationEntity, ReservationCreateDto>().ReverseMap();
        }
    }
}
