using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;

namespace RMall.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public RoomController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllRoom()
        {
            try
            {
                List<Room> rooms = await _context.Rooms.ToListAsync();
                List<RoomDTO> result = new List<RoomDTO>();
                foreach (var room in rooms)
                {
                    result.Add(new RoomDTO
                    {
                        id = room.Id,
                        name = room.Name,
                        slug = room.Slug,
                        rows = room.Rows,
                        columns = room.Columns,
                        createdAt = room.CreatedAt,
                        updatedAt = room.UpdatedAt,
                        deletedAt = room.DeletedAt,
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
