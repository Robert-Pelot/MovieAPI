using MovieApi.Dtos;
using MovieApi.Repositories;

namespace MovieApi.Services;

public interface IMovieService
{
    IReadOnlyCollection<MovieResponse> GetMovies(int? year = null);
    MovieResponse? GetMovie(string name);
    bool CreateMovie(MovieRequest request, out MovieResponse movie);
    UpdateResult UpdateMovie(string name, MovieRequest request);
    bool DeleteMovie(string name);
}
