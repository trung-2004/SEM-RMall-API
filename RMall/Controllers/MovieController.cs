using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.Galleries;
using RMall.Models.General;
using RMall.Models.MovieGenre;
using RMall.Models.MovieLanguage;
using RMall.Models.Movies;
using RMall.Service.Movies;
using RMall.Service.UploadFiles;
using System.IO;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RMall.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IImgService _imgService;
        private readonly IMovieService _movieService;

        public MovieController(RmallApiContext context, IImgService imgService, IMovieService movieService)
        {
            _context = context;
            _imgService = imgService;
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieAll()
        {
            try
            {
                List<Movie> movies = await _context.Movies.Include(m => m.MovieGenres).ThenInclude(m => m.Genre).Include(m => m.MovieLanguages).ThenInclude(m => m.Language).Where(m => m.DeletedAt == null).OrderByDescending(m => m.Id).ToListAsync();
                List<MovieDTO> result = new List<MovieDTO>();
                foreach (Movie m in movies)
                {
                    var movieDto = new MovieDTO
                    {
                        id = m.Id,
                        title = m.Title,
                        actor = m.Actor,
                        movie_image = m.MovieImage,
                        cover_image = m.CoverImage,
                        describe = m.Describe,
                        director = m.Director,
                        duration = m.Duration,
                        favoriteCount = m.FavoriteCount,
                        trailer = m.Trailer,
                        release_date = m.ReleaseDate,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
                    };

                    var genres = new List<GenreMovieResponse>();
                    var languages = new List<LanguageMovieResponse>();

                    foreach (var item in m.MovieGenres)
                    {
                        var genre = new GenreMovieResponse
                        {
                            id = item.Id,
                            name = item.Genre.Name
                        };
                        genres.Add(genre);
                        movieDto.genres = genres;
                    }

                    foreach (var item in m.MovieLanguages)
                    {
                        var language = new LanguageMovieResponse
                        {
                            id = item.Id,
                            name = item.Language.Name
                        };
                        languages.Add(language);
                        movieDto.languages = languages;
                    }

                    var ticketCount = await _context.Tickets.Include(t => t.Order).ThenInclude(t => t.Show).ThenInclude(t => t.Movie).Where(t => t.Order.Show.Movie.Id == m.Id).CountAsync();

                    movieDto.totalTicket = ticketCount;

                    result.Add(movieDto);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                Movie m = await _context.Movies.Include(m => m.MovieGenres).ThenInclude(m=>m.Genre).Include(m => m.MovieLanguages).ThenInclude(m => m.Language).FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
                if (m != null)
                {
                    var movieDto = new MovieDTO
                    {
                        id = m.Id,
                        title = m.Title,
                        actor = m.Actor,
                        movie_image = m.MovieImage,
                        cover_image = m.CoverImage,
                        describe = m.Describe,
                        director = m.Director,
                        duration = m.Duration,
                        favoriteCount = m.FavoriteCount,
                        trailer = m.Trailer,
                        release_date = m.ReleaseDate,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
                    };

                    var genres = new List<GenreMovieResponse>();
                    var languages = new List<LanguageMovieResponse>();
                    var galleries = new List<GaleryResponse>();

                    foreach (var item in m.MovieGenres)
                    {
                        var genre = new GenreMovieResponse
                        {
                            id = item.Id,
                            name = item.Genre.Name
                        };
                        genres.Add(genre);
                    }

                    movieDto.genres = genres;

                    foreach (var item in m.MovieLanguages)
                    {
                        var language = new LanguageMovieResponse
                        {
                            id = item.Id,
                            name = item.Language.Name
                        };
                        languages.Add(language);
                    }

                    movieDto.languages = languages;

                    var gallerys = _context.GalleryMovies.Where(g => g.MovieId == movieDto.id).ToList();

                    foreach (var item in gallerys)
                    {
                        var gallery = new GaleryResponse
                        {
                            id = item.Id,
                            imagePath = item.ImagePath,
                        };
                        galleries.Add(gallery);
                    }
                    movieDto.galeries = galleries;

                    movieDto.favoriteCount = await _context.Favorites.Where(f => f.MovieId == m.Id).CountAsync();

                    var ticketCount = await _context.Tickets.Include(t => t.Order).ThenInclude(t => t.Show).ThenInclude(t => t.Movie).Where(t => t.Order.Show.Movie.Id == m.Id).CountAsync();

                    movieDto.totalTicket = ticketCount;

                    return Ok(movieDto);
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
            return NotFound();
        }

        [HttpGet("trash-can")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]

        public async Task<IActionResult> TrashCan()
        {
            try
            {
                List<Movie> movies = await _context.Movies.Include(m => m.MovieGenres).ThenInclude(m => m.Genre).Include(m => m.MovieLanguages).ThenInclude(m => m.Language).Where(m => m.DeletedAt != null).OrderByDescending(m => m.DeletedAt).ToListAsync();
                List<MovieDTO> result = new List<MovieDTO>();
                foreach (Movie m in movies)
                {
                    var movieDto = new MovieDTO
                    {
                        id = m.Id,
                        title = m.Title,
                        actor = m.Actor,
                        movie_image = m.MovieImage,
                        cover_image = m.CoverImage,
                        describe = m.Describe,
                        director = m.Director,
                        duration = m.Duration,
                        favoriteCount = m.FavoriteCount,
                        trailer = m.Trailer,
                        release_date = m.ReleaseDate,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
                    };

                    var genres = new List<GenreMovieResponse>();
                    var languages = new List<LanguageMovieResponse>();

                    foreach (var item in m.MovieGenres)
                    {
                        var genre = new GenreMovieResponse
                        {
                            id = item.Id,
                            name = item.Genre.Name
                        };
                        genres.Add(genre);
                        movieDto.genres = genres;
                    }

                    foreach (var item in m.MovieLanguages)
                    {
                        var language = new LanguageMovieResponse
                        {
                            id = item.Id,
                            name = item.Language.Name
                        };
                        languages.Add(language);
                        movieDto.languages = languages;
                    }

                    result.Add(movieDto);
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
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> CreateMovie([FromForm]CreateMovie model)
        {
            try
            {
                bool movieExists = await _context.Movies.AnyAsync(c => c.Title.Equals(model.title));

                if (movieExists)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Movie already exists",
                        Data = ""
                    });
                }

                var imageUrl = await _imgService.UploadImageAsync(model.movie_image, "movies");
                var coverUrl = await _imgService.UploadImageAsync(model.cover_image, "movies");

                if (imageUrl != null && coverUrl != null)
                {
                    Movie m = new Movie
                    {
                        Title = model.title,
                        Actor = model.actor,
                        MovieImage = imageUrl,
                        CoverImage = coverUrl,
                        Describe = model.describe,
                        Director = model.director,
                        Duration = model.duration,
                        FavoriteCount = 0,
                        Trailer = model.trailer,
                        ReleaseDate = model.release_date,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    _context.Movies.Add(m);
                    await _context.SaveChangesAsync();

                    foreach (var genreId in model.genreIds)
                    {
                        var movieGenre = new MovieGenre
                        {
                            MovieId = m.Id,
                            GenreId = genreId,
                        };

                        _context.MovieGenres.Add(movieGenre);
                        await _context.SaveChangesAsync();
                    }

                    foreach (var languageId in model.languageIds)
                    {
                        var movieLanguage = new MovieLanguage
                        {
                            MovieId = m.Id,
                            LanguageId = languageId,
                        };

                        _context.MovieLanguages.Add(movieLanguage);
                        await _context.SaveChangesAsync();
                    }

                    return Created($"get-by-id?id={m.Id}", new MovieDTO
                    {
                        id = m.Id,
                        title = m.Title,
                        actor = m.Actor,
                        movie_image = m.MovieImage,
                        describe = m.Describe,
                        director = m.Director,
                        duration = m.Duration,
                        favoriteCount = m.FavoriteCount,
                        trailer = m.Trailer,
                        release_date = m.ReleaseDate,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        deletedAt = m.DeletedAt,
                    });
                }
                else
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Please provide an avatar.",
                        Data = ""
                    });
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

        [HttpPut("edit")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> EditMovie([FromForm]EditMovie model)
        {
            try
            {
                Movie existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.id);
                if (existingMovie != null)
                {
                    Movie movie = new Movie
                    {
                        Id = model.id,
                        Title = model.title,
                        Actor = model.actor,
                        Describe = model.describe,
                        Director = model.director,
                        Duration = model.duration,
                        FavoriteCount = existingMovie.FavoriteCount,
                        Trailer = model.trailer,
                        ReleaseDate = model.release_date,
                        CreatedAt = existingMovie.CreatedAt,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    if (model.movie_image != null)
                    {
                        string imageUrl = await _imgService.UploadImageAsync(model.movie_image, "movies");

                        if (imageUrl == null)
                        {
                            return BadRequest(new GeneralServiceResponse
                            {
                                Success = false,
                                StatusCode = 400,
                                Message = "Failed to upload avatar.",
                                Data = ""
                            });
                        }

                        movie.MovieImage = imageUrl;
                    }
                    else
                    {
                        movie.MovieImage = existingMovie.MovieImage;
                    }

                    if (model.cover_image != null)
                    {
                        string imageUrl = await _imgService.UploadImageAsync(model.cover_image, "movies");

                        if (imageUrl == null)
                        {
                            return BadRequest(new GeneralServiceResponse
                            {
                                Success = false,
                                StatusCode = 400,
                                Message = "Failed to upload avatar.",
                                Data = ""
                            });
                        }

                        movie.CoverImage = imageUrl;
                    }
                    else
                    {
                        movie.CoverImage = existingMovie.MovieImage;
                    }

                    _context.Movies.Update(movie);
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

        [HttpDelete("delete")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> SoftDelete(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    Movie movie = await _context.Movies.FindAsync(id);

                    if (movie != null)
                    {
                        movie.DeletedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Soft delete successful",
                    Data = null
                };

                return Ok(response);


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

        [HttpPut]
        [Route("restore/{id}")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                Movie movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }

                movie.DeletedAt = null;

                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "restore successful",
                    Data = null
                };

                return Ok(response);

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

        [HttpGet("comming-soon")]
        public async Task<IActionResult> GetMovieCommingSoon()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();

                var currentDateTime = DateTime.Now;
                List<MovieDTO> upcomingMovies = movies.Where(movies => movies.release_date > currentDateTime).Take(6).ToList();

                return Ok(upcomingMovies);

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

        [HttpGet("last-showing")]
        public async Task<IActionResult> GetMovieLastShowing()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();

                var currentDateTime = DateTime.Now;
                List<MovieDTO> lastShowingMovies = movies.Where(movies => movies.release_date <= currentDateTime).Take(6).ToList();
                return Ok(lastShowingMovies);

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

        [HttpGet("best-showing")]
        public async Task<IActionResult> GetMovieBestShowing()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();

                var currentDateTime = DateTime.Now;
                List<MovieDTO> lastShowingMovies = movies.Where(movies => movies.release_date <= currentDateTime).Take(6).ToList();
                return Ok(lastShowingMovies);

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
