using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.Movies;

namespace RMall.Service.Movies
{
    public class MovieService : IMovieService
    {
        private readonly RmallApiContext _context;
        public MovieService(RmallApiContext context) 
        {
            _context = context;
        }

        public async Task<List<MovieDTO>> GetAllMoviesAsync()
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
                    director = m.Director,
                    duration = m.Duration,
                    ratings = m.Ratings,
                    trailer = m.Trailer,
                    cast = m.Cast,
                    release_date = m.ReleaseDate,
                    createdAt = m.CreatedAt,
                    updatedAt = m.UpdatedAt,
                    deletedAt = m.DeletedAt,
                });
            }
            return result;
        }

        public Task<MovieDTO> GetMovieByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MovieDTO> CreateMovieAsync(CreateMovie model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMovieAsync(EditMovie model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMovieAsync(int id)
        {
            throw new NotImplementedException();
        }    
    }
}
