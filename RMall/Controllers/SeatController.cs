using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Seats;

namespace RMall.Controllers
{
    [Route("api/seat")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public SeatController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet("get-by-roomId/{id}")]
        public async Task<IActionResult> GetAllSeatByRoom(int id)
        {
            try
            {
                List<Seat> seats = await _context.Seats.Where(s => s.RoomId == id).OrderByDescending(s => s.SeatTypeId).ToListAsync();
                List<SeatDTO> result = new List<SeatDTO>();
                foreach (var seat in seats)
                {
                    result.Add(new SeatDTO
                    {
                        id = seat.Id,
                        roomId = seat.RoomId,
                        seatTypeId = seat.SeatTypeId,
                        rowNumber = seat.RowNumber,
                        seatNumber = seat.SeatNumber,
                        createdAt = seat.CreatedAt,
                        updatedAt = seat.UpdatedAt,
                        deletedAt = seat.DeletedAt,
                    });
                }
                return Ok(result);
            } catch (Exception ex)
            {
                var response = new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = ""
                };

                return BadRequest(response);
            }
        }

        private async Task<decimal> PriceSeat(int showId, int seatTypeId)
        {
            var price = await _context.SeatPricings.FirstOrDefaultAsync(s => s.SeatTypeId == seatTypeId && s.ShowId == showId);
            if (price == null)
            {
                return 0;
            }
            return price.Price;
        }

        [HttpGet("get-by-showCode/{ShowCode}")]
        public async Task<IActionResult> GetAllSeatByShow(string ShowCode)
        {
            try
            {
                var show = await _context.Shows.Include(s => s.Orders).ThenInclude(s => s.Tickets).FirstOrDefaultAsync(s => s.ShowCode.Equals(ShowCode) && s.DeletedAt == null);
                if (show == null)
                {
                    return NotFound();
                }
                List<Seat> seats = await _context.Seats
                    .Where(s => s.RoomId == show.RoomId)
                    .OrderByDescending(s => s.SeatTypeId)
                    .ToListAsync();

                List<int> seatsBooked = show.Orders
                    .SelectMany(o => o.Tickets.Select(t => t.SeatId))
                    .ToList();

                var seatPricings = await _context.SeatPricings
                    .Where(sp => sp.ShowId == show.Id)
                    .ToListAsync();

                List<SeatResponse> result = seats.Select(seat =>
                {
                    var seatPricing = seatPricings.FirstOrDefault(sp => sp.SeatTypeId == seat.SeatTypeId);
                    decimal price = seatPricing != null ? seatPricing.Price : 0;
                    return new SeatResponse
                    {
                        id = seat.Id,
                        roomId = seat.RoomId,
                        seatTypeId = seat.SeatTypeId,
                        rowNumber = seat.RowNumber,
                        seatNumber = seat.SeatNumber,
                        createdAt = seat.CreatedAt,
                        updatedAt = seat.UpdatedAt,
                        deletedAt = seat.DeletedAt,
                        isBooked = seatsBooked.Contains(seat.Id),
                        price = price,
                    };
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var response = new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = ""
                };

                return BadRequest(response);
            }
        }
    }
}
