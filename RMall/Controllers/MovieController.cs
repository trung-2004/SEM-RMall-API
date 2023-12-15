using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;

namespace RMall.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly RmallApiContext _context;

        public MovieController(RmallApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieAll()
        {
            try
            {
                List<Movie> movies = await _context.Movies.Where(m => m.DeletedAt == null).OrderByDescending(m => m.Id).ToListAsync();
                List<MovieDTO> result = new List<MovieDTO>();
                foreach (Movie m in movies)
                {
                    result.Add(new MovieDTO
                    {
                        id = m.Id,
                        title = m.Title,
                        actor = m.Actor,
                        movie_image = m.MovieImage,
                        describe = m.Describe,
                        mirector = m.Director,
                        duration = m.Duration,
                        language = m.Language,
                        ratings = m.Ratings,
                        trailer = m.Trailer,
                        cast = m.Cast,
                        release_date = m.ReleaseDate,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateMovie()
        {

        }
    }
}
