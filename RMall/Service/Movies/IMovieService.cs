using RMall.DTOs;
using RMall.Models.Movies;

namespace RMall.Service.Movies
{
    public interface IMovieService
    {
        Task<List<MovieDTO>> GetAllMoviesAsync();
        Task<MovieDTO> GetMovieByIdAsync(int id);
        Task<MovieDTO> CreateMovieAsync(CreateMovie model);
        Task<bool> UpdateMovieAsync(EditMovie model);
        Task<bool> DeleteMovieAsync(int id);
    }
}
