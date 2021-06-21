using System;
using System.Linq;
using AutoMapper;
using SGReservation.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SGReservation.Repositories;
using System.Collections.Generic;
using SGReservation.Entities;
using SGReservation.Models;
using SGReservation.Helpers;
using System.Text.Json;

namespace SGReservation.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public ReservationsController(
            IUrlHelper urlHelper,
            IReservationRepository reservationRepository,
            IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = nameof(GetAllReservations))]
        public ActionResult GetAllReservations(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<ReservationEntity> reservationItems = _reservationRepository.GetAll(queryParameters).ToList();

            var allItemCount = _reservationRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = reservationItems.Select(x => ExpandSingleReservationItem(x, version));

            //return Ok(reservationItems.OrderBy(x => x.Room));
            return Ok(toReturn);
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleReservation))]
        public ActionResult GetSingleReservation(ApiVersion version, int id)
        {
            ReservationEntity reservationItem = _reservationRepository.GetSingle(id);

            if (reservationItem == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleReservationItem(reservationItem, version));
        }

        [HttpPost(Name = nameof(AddReservation))]
        public ActionResult<ReservationDto> AddReservation(ApiVersion version, [FromBody] ReservationCreateDto reservationCreateDto)
        {
            if (reservationCreateDto == null)
            {
                return BadRequest();
            }

            ReservationEntity toAdd = _mapper.Map<ReservationEntity>(reservationCreateDto);

            _reservationRepository.Add(toAdd);

            if (!_reservationRepository.Save())
            {
                throw new Exception("Creating a reservation item failed on save.");
            }

            ReservationEntity newReservationItem = _reservationRepository.GetSingle(toAdd.Id);

            return CreatedAtRoute(nameof(GetSingleReservation),
                new { version = version.ToString(), id = newReservationItem.Id },
                _mapper.Map<ReservationDto>(newReservationItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateReservation))]
        public ActionResult<ReservationDto> PartiallyUpdateReservation(int id, [FromBody] JsonPatchDocument<ReservationUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            ReservationEntity existingEntity = _reservationRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            ReservationUpdateDto reservationUpdateDto = _mapper.Map<ReservationUpdateDto>(existingEntity);
            patchDoc.ApplyTo(reservationUpdateDto);

            TryValidateModel(reservationUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(reservationUpdateDto, existingEntity);
            ReservationEntity updated = _reservationRepository.Update(id, existingEntity);

            if (!_reservationRepository.Save())
            {
                throw new Exception("Updating a Reservation Item failed on save.");
            }

            return Ok(_mapper.Map<ReservationDto>(updated));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveReservation))]
        public ActionResult RemoveReservation(int id)
        {
            ReservationEntity reservationItem = _reservationRepository.GetSingle(id);

            if (reservationItem == null)
            {
                return NotFound();
            }

            _reservationRepository.Delete(id);

            if (!_reservationRepository.Save())
            {
                throw new Exception("Deleting a Reservation Item failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateReservation))]
        public ActionResult<ReservationDto> UpdateReservation(int id, [FromBody] ReservationUpdateDto reservationUpdateDto)
        {
            if (reservationUpdateDto == null)
            {
                return BadRequest();
            }

            var existingReservationItem = _reservationRepository.GetSingle(id);

            if (existingReservationItem == null)
            {
                return NotFound();
            }

            _mapper.Map(reservationUpdateDto, existingReservationItem);

            _reservationRepository.Update(id, existingReservationItem);

            if (!_reservationRepository.Save())
            {
                throw new Exception("Updating a Reservation Item failed on save.");
            }

            return Ok(_mapper.Map<ReservationDto>(existingReservationItem));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            ICollection<ReservationEntity> reservationtems = _reservationRepository.GetRandomMeal();

            IEnumerable<ReservationDto> dtos = reservationtems
                .Select(x => _mapper.Map<ReservationDto>(x));

            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetRandomMeal), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllReservations), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllReservations), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllReservations), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllReservations), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllReservations), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddReservation), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_reservation",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleReservationItem(ReservationEntity reservationItem, ApiVersion version)
        {
            var links = GetLinks(reservationItem.Id, version);
            ReservationDto item = _mapper.Map<ReservationDto>(reservationItem);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleReservation), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveReservation), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_reservation",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddReservation), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_reservation",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateReservation), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_reservation",
               "PUT"));

            return links;
        }
    }
}
