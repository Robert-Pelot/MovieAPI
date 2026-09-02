using MovieApi.Models;

namespace MovieApi.Repositories;

public interface IMovieRepository
{
    IReadOnlyCollection<Movie> GetAll();
    Movie? GetByName(string name);
    bool TryAdd(Movie movie);
    UpdateResult Update(string currentName, Movie movie);
    bool Delete(string name);
}

public enum UpdateResult
{
    Updated,
    NotFound,
    DuplicateName,
}
