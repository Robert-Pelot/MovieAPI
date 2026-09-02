using MovieApi.Dtos;
using MovieApi.Models;
using MovieApi.Repositories;

namespace MovieApi.Services;

public sealed class MovieService(IMovieRepository repository) : IMovieService
{
    public IReadOnlyCollection<MovieResponse> GetMovies(int? year = null) => repository
        .GetAll()
        .Where(movie => year is null || movie.Year == year)
        .Select(ToResponse)
        .ToArray();

    public MovieResponse? GetMovie(string name)
    {
        var movie = repository.GetByName(name.Trim());
        return movie is null ? null : ToResponse(movie);
    }

    public bool CreateMovie(MovieRequest request, out MovieResponse movie)
    {
        var entity = ToMovie(request);
        movie = ToResponse(entity);
        return repository.TryAdd(entity);
    }

    public UpdateResult UpdateMovie(string name, MovieRequest request) =>
        repository.Update(name.Trim(), ToMovie(request));

    public bool DeleteMovie(string name) => repository.Delete(name.Trim());

    private static Movie ToMovie(MovieRequest request) =>
        new(request.Name.Trim(), request.Genre.Trim(), request.Year);

    private static MovieResponse ToResponse(Movie movie) =>
        new(movie.Name, movie.Genre, movie.Year);
}
