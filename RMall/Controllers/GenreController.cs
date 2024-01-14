using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Genre;

namespace RMall.Controllers
{
    [Route("api/genre")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public GenreController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetGenreAll()
        {
            try
            {
                List<Genre> genres = await _context.Genres.Where(m => m.DeletedAt == null).OrderBy(m => m.Id).ToListAsync();
                List<GenreDTO> result = new List<GenreDTO>();
                foreach (Genre m in genres)
                {
                    result.Add(new GenreDTO
                    {
                        id = m.Id,
                        name = m.Name,
                        slug = m.Slug,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
                    });
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

        [HttpGet("trash-can")]
        [Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> TrashCan()
        {
            try
            {
                List<Genre> genres = await _context.Genres.Where(m => m.DeletedAt != null).OrderByDescending(m => m.DeletedAt).ToListAsync();
                List<GenreDTO> result = new List<GenreDTO>();
                foreach (Genre m in genres)
                {
                    result.Add(new GenreDTO
                    {
                        id = m.Id,
                        name = m.Name,
                        slug = m.Slug,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
                    });
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

        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> getGenreById(int id)
        {
            try
            {
                Genre genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id && g.DeletedAt == null);
                if (genre != null)
                {
                    return Ok(new GenreDTO
                    {
                        id = genre.Id,
                        name = genre.Name,
                        slug = genre.Slug,
                        createdAt = genre.CreatedAt,
                        updatedAt = genre.UpdatedAt,
                        deletedAt = genre.DeletedAt,    
                    });
                }
                else
                {
                    var response = new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    };

                    return NotFound(response);
                }
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
        [Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> CreateGenre(CreateGenre model)
        {
            try
            {
                bool genreExists = await _context.Genres.AnyAsync(c => c.Name.Equals(model.name));

                if (genreExists)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Genre already exists",
                        Data = ""
                    });
                }

                Genre m = new Genre
                {
                    Name = model.name,
                    Slug = model.name.ToLower().Replace(" ", "-"),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                };

                _context.Genres.Add(m);
                await _context.SaveChangesAsync();

                return Created($"get-by-id?id={m.Id}", new GenreDTO
                {
                    id = m.Id,
                    name = m.Name,
                    slug = m.Slug,
                    createdAt = m.CreatedAt,
                    updatedAt = m.UpdatedAt,
                    deletedAt = m.DeletedAt,
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

        [HttpPut("edit")]
        [Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> EditGenre(EditGenre model)
        {
            try
            {
                Genre existingGenre = await _context.Genres.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.id);
                if (existingGenre != null)
                {
                    Genre genre = new Genre
                    {
                        Id = model.id,
                        Name = model.name,
                        Slug = model.name.ToLower().Replace(" ", "-"),
                        CreatedAt = existingGenre.CreatedAt,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    _context.Genres.Update(genre);
                    await _context.SaveChangesAsync();

                    return Ok(new GeneralServiceResponse
                    {
                        Success = true,
                        StatusCode = 200,
                        Message = "Edit successfully",
                        Data = ""
                    });
                }
                else
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }
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

        [HttpDelete("delete")]
        [Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> SoftDelete(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    Genre genre = await _context.Genres.FindAsync(id);

                    if (genre != null)
                    {
                        genre.DeletedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Soft delete successful",
                    Data = ""
                };

                return Ok(response);


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

        [HttpPut]
        [Route("restore/{id}")]
        [Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                Genre genre = await _context.Genres.FindAsync(id);
                if (genre == null)
                {
                    return NotFound();
                }

                genre.DeletedAt = null;

                _context.Genres.Update(genre);
                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "restore successful",
                    Data = ""
                };

                return Ok(response);

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
