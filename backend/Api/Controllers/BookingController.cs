using Microsoft.AspNetCore.Mvc;
using Api.Repositories;
using Api.models.neo4j.Nodes;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingRepository _repo;
    public BookingController(BookingRepository repo) => _repo = repo;

    // Kreiranje kompletne rezervacije
    [HttpPost]
    public async Task<IActionResult> BookFlight(int passengerId, int routeId, string seat, double weight, int count, double seatPrice, double suitcasePrice)
    {
        try {
            await _repo.CreateBookingAsync(passengerId, routeId, seat, weight, count, seatPrice, suitcasePrice);
            return Ok("Rezervacija uspešna. Kreirani su karta, račun i prtljag.");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

// PUT: api/Booking/{ticketId}
[HttpPut("{ticketId}")]
public async Task<IActionResult> UpdateBooking(
    int ticketId, 
    [FromQuery] string newSeat, 
    [FromQuery] double newWeight, 
    [FromQuery] int newCount, 
    [FromQuery] double newSeatPrice, 
    [FromQuery] double newSuitcasePrice)
{
    try 
    {
        await _repo.UpdateBookingAsync(ticketId, newSeat, newWeight, newCount, newSeatPrice, newSuitcasePrice);
        return Ok("Rezervacija je kompletno ažurirana (sedište, prtljag i račun).");
    }
    catch (Exception ex) 
    {
        return BadRequest($"Greška pri ažuriranju: {ex.Message}");
    }
}

    // Otkazivanje
    [HttpDelete("{ticketId}")]
    public async Task<IActionResult> CancelBooking(int ticketId)
    {
        await _repo.CancelBookingAsync(ticketId);
        return Ok("Rezervacija otkazana i svi povezani dokumenti obrisani.");
    }
}