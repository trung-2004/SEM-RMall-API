using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;

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
    }
}
