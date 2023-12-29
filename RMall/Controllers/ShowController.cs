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
using static System.Net.Mime.MediaTypeNames;

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
                    Language = model.language,
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

                return Created($"get-by-id?id={show.Id}", new ShowDTO
                {
                    id = show.Id,
                    movieId = show.MovieId,
                    roomId = show.RoomId,
                    showCode = show.ShowCode,
                    startDate = show.StartDate,
                    createdAt = show.CreatedAt,
                    updatedAt = show.UpdatedAt,
                    deletedAt = show.DeletedAt,
                });
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

        [HttpGet("get-by-movie/{id}")]
        public async Task<IActionResult> GetShowByMovie(int id, DateTime? from, string? language)
        {
            try
            {
                List<ShowDTO> result = new List<ShowDTO>();

                DateTime currentDate = DateTime.Now.Date;
                DateTime endDate = currentDate.AddDays(6);
                //var shows = await _context.Shows.Where(s => s.StartDate >= DateTime.Now && s.MovieId == id && s.StartDate <= DateTime.Now.AddDays(6)).OrderBy(s => s.StartDate).ToListAsync();
                var shows = _context.Shows.Where(s => s.StartDate >= currentDate && s.MovieId == id && s.StartDate <= endDate).OrderBy(s => s.StartDate).AsQueryable();
                
                #region Filtering
                if (!string.IsNullOrEmpty(language))
                {
                    shows = shows.Where(s => s.Language.Contains(language));
                }
                if (from.HasValue)
                {
                    shows = shows.Where(s => s.StartDate.Date == from.Value.Date);
                }
                #endregion

                foreach (var show in shows)
                {
                    result.Add(new ShowDTO
                    {
                        id = show.Id,
                        movieId = show.MovieId,
                        roomId = show.RoomId,
                        startDate = show.StartDate,
                        showCode = show.ShowCode,
                        language = show.Language,
                        createdAt = show.CreatedAt,
                        updatedAt = show.UpdatedAt,
                        deletedAt = show.DeletedAt,
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
