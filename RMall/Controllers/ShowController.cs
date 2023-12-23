using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.MovieGenre;
using RMall.Models.MovieLanguage;
using RMall.Models.SeatPricings;
using RMall.Models.Shows;

namespace RMall.Controllers
{
    [Route("api/show")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public ShowController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetShowAll()
        {
            try
            {
                List<Show> shows = await _context.Shows.Include(s => s.Movie).Include(s => s.Room).Include(m => m.SeatPricings).ThenInclude(m => m.SeatType).Where(m => m.DeletedAt == null).OrderByDescending(m => m.Id).ToListAsync();
                List<ShowDTO> result = new List<ShowDTO>();
                foreach (Show s in shows)
                {
                    var showDto = new ShowDTO
                    {
                        id = s.Id,
                        movieId = s.MovieId,
                        roomId = s.RoomId,
                        movieName = s.Movie.Title,
                        roomName = s.Room.Name,
                        startDate = s.StartDate,
                        showCode = s.ShowCode,
                        createdAt = s.CreatedAt,
                        updatedAt = s.UpdatedAt,
                        deletedAt = s.DeletedAt,
                    };

                    var seatPricings = new List<SeatPricingResponse>();

                    foreach (var item in s.SeatPricings)
                    {
                        var seatPricing = new SeatPricingResponse
                        {
                            id = item.Id,
                            showId = item.ShowId,
                            seatTypeId = item.SeatTypeId,
                            seatTypeName = item.SeatType.Name,
                            price = item.Price,
                        };
                        seatPricings.Add(seatPricing);
                        showDto.seatPricings = seatPricings;
                    }

                    result.Add(showDto);
                }
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

        [HttpPost]
        public async Task<IActionResult> CreateShow(CreateShow model)
        {
            try
            {
                if (model.startDate < DateTime.Now)
                {
                    var response = new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid time",
                        Data = ""
                    };

                    return BadRequest(response);
                }
                Show show = new Show
                {
                    MovieId = model.movieId,
                    RoomId = model.roomId,
                    ShowCode = model.showCode,
                    StartDate = model.startDate,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null
                };

                _context.Shows.Add(show);
                await _context.SaveChangesAsync();

                foreach (var item in model.seatPricings)
                {
                    var seatPricing = new SeatPricing
                    {
                        ShowId = show.Id,
                        SeatTypeId = item.seatTypeId,
                        Price = item.price,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null
                    };

                    _context.SeatPricings.Add(seatPricing);
                    await _context.SaveChangesAsync();
                }

                return Ok(model);
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

        [HttpGet("get-by-movie")]
        public async Task<IActionResult> GetShowByMovie()
        {
            try
            {

                return Ok();
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
