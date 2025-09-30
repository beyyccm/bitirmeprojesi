using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UcakRezervasyon.Entities.Models;
using UcakRezervasyon.DataAccess.Repositories;

namespace UcakRezervasyon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public FlightsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var flights = await _uow.Flights.GetAllAsync();
        return Ok(flights);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Flight flight)
    {
        await _uow.Flights.AddAsync(flight);
        await _uow.CompleteAsync();
        return CreatedAtAction(nameof(GetAll), new { id = flight.Id }, flight);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Flight flight)
    {
        var existing = await _uow.Flights.GetAsync(id);
        if (existing == null) return NotFound();
        existing.FlightNumber = flight.FlightNumber;
        existing.DepartureAirport = flight.DepartureAirport;
        existing.ArrivalAirport = flight.ArrivalAirport;
        existing.DepartureTime = flight.DepartureTime;
        existing.ArrivalTime = flight.ArrivalTime;
        existing.Price = flight.Price;
        existing.Capacity = flight.Capacity;
        _uow.Flights.Update(existing);
        await _uow.CompleteAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _uow.Flights.GetAsync(id);
        if (existing == null) return NotFound();
        _uow.Flights.Remove(existing);
        await _uow.CompleteAsync();
        return NoContent();
    }
}
