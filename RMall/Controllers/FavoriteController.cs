using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.Favorites;
using RMall.Models.General;
using System.Security.Claims;
using System.Security.Principal;

namespace RMall.Controllers
{
    [Route("api/favorite")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public FavoriteController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet("get-by-user")]
        [Authorize]
        public async Task<IActionResult> GetByUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
            }
            try
            {
                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                }

                List<Favorite> favorites = await _context.Favorites.Include(f => f.Movie).Where(f => f.UserId == user.Id).OrderByDescending(p => p.Id).ToListAsync();
                List<FavoriteDTO> result = new List<FavoriteDTO>();
                foreach (var item in favorites)
                {
                    result.Add(new FavoriteDTO
                    {
                        id = item.Id,
                        movieId = item.MovieId,
                        movieName = item.Movie.Title,
                        userId = item.UserId,
                        createdAt = item.CreatedAt,
                        updatedAt = item.UpdatedAt,
                        deletedAt = item.DeletedAt,
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

        [HttpPost("add-to-favorite")]
        [Authorize]
        public async Task<IActionResult> addToFavorite(CreateFavorite model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Not Authorized",
                    Data = ""
                });
            }
            try
            {
                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                }

                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == user.Id && f.MovieId == model.movieId);

                if (existingFavorite != null)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 401,
                        Message = "The movie already exists in the favorites list.",
                        Data = ""
                    });
                }

                Favorite favorite = new Favorite
                {
                    UserId = user.Id,
                    MovieId = model.movieId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                var movie = await _context.Movies.FindAsync(model.movieId);
                movie.FavoriteCount = await _context.Favorites.Where(f => f.MovieId == movie.Id).CountAsync();

                await _context.SaveChangesAsync();

                return Created($"get-by-id?id={favorite.Id}", new FavoriteDTO
                {
                    id = favorite.Id,
                    movieId = model.movieId,
                    userId = user.Id,
                    createdAt = favorite.CreatedAt,
                    updatedAt = favorite.UpdatedAt,
                    deletedAt = favorite.DeletedAt,
                });

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

        [HttpDelete("remove-from-favorite")]
        [Authorize]
        public async Task<IActionResult> removeFromFavorite(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
            }
            try
            {
                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                }

                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == user.Id && f.Id == id);

                if (favorite == null)
                {
                    return BadRequest(new GeneralServiceResponse { Success = false, StatusCode = 400, Message = "The movie does not exist in the favorites list.", Data = "" });
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                var movie = await _context.Movies.FindAsync(favorite.MovieId);
                movie.FavoriteCount = await _context.Favorites.Where(f => f.MovieId == movie.Id).CountAsync();

                await _context.SaveChangesAsync();

                return Ok(new GeneralServiceResponse { Success = true, StatusCode = 200, Message = "removed from favorites list", Data = "" });

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
