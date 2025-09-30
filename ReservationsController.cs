using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcakRezervasyon.Entities.Models;
using UcakRezervasyon.DataAccess.Repositories;

namespace UcakRezervasyon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public ReservationsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        decimal total = 0;
        var reservation = new Reservation {
            CustomerId = dto.CustomerId,
            ReservationDate = DateTime.UtcNow
        };
        await _uow.Reservations.AddAsync(reservation);
        await _uow.CompleteAsync();

        foreach (var item in dto.Items)
        {
            var flight = await _uow.Flights.GetAsync(item.FlightId);
            if (flight == null) return BadRequest(new { message = $"Flight {item.FlightId} not found" });
            if (flight.Capacity < item.SeatCount) return BadRequest(new { message = $"Not enough capacity for flight {flight.FlightNumber}" });
            flight.Capacity -= item.SeatCount;
            _uow.Flights.Update(flight);

            var rf = new ReservationFlight {
                ReservationId = reservation.Id,
                FlightId = flight.Id,
                SeatCount = item.SeatCount
            };
            await _uow.ReservationFlights.AddAsync(rf);
            total += flight.Price * item.SeatCount;
        }

        reservation.TotalAmount = total;
        _uow.Reservations.Update(reservation);
        await _uow.CompleteAsync();

        return Ok(reservation);
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var list = await _uow.Reservations.FindAsync(r => r.CustomerId == userId);
        return Ok(list);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _uow.Reservations.GetAllAsync();
        return Ok(list);
    }
}

public record CreateReservationDto(int CustomerId, List<CreateReservationItem> Items);
public record CreateReservationItem(int FlightId, int SeatCount);
