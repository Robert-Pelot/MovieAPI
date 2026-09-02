using MovieApi.Models;

namespace MovieApi.Repositories;

public sealed class MovieRepository : IMovieRepository
{
    private readonly Dictionary<string, Movie> _movies = new(StringComparer.OrdinalIgnoreCase);
    private readonly ReaderWriterLockSlim _lock = new();

    public MovieRepository()
    {
        foreach (var movie in SeedMovies)
        {
            _movies.Add(movie.Name, movie);
        }
    }

    public IReadOnlyCollection<Movie> GetAll()
    {
        _lock.EnterReadLock();
        try
        {
            return _movies.Values.OrderBy(movie => movie.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public Movie? GetByName(string name)
    {
        _lock.EnterReadLock();
        try
        {
            return _movies.GetValueOrDefault(name);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public bool TryAdd(Movie movie)
    {
        _lock.EnterWriteLock();
        try
        {
            return _movies.TryAdd(movie.Name, movie);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public UpdateResult Update(string currentName, Movie movie)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_movies.ContainsKey(currentName))
            {
                return UpdateResult.NotFound;
            }

            if (!currentName.Equals(movie.Name, StringComparison.OrdinalIgnoreCase) && _movies.ContainsKey(movie.Name))
            {
                return UpdateResult.DuplicateName;
            }

            _movies.Remove(currentName);
            _movies.Add(movie.Name, movie);
            return UpdateResult.Updated;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool Delete(string name)
    {
        _lock.EnterWriteLock();
        try
        {
            return _movies.Remove(name);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private static readonly Movie[] SeedMovies =
    [
        new("The Shawshank Redemption", "Drama", 1994),
        new("The Godfather", "Crime", 1972),
        new("The Dark Knight", "Action", 2008),
        new("Pulp Fiction", "Crime", 1994),
        new("Forrest Gump", "Drama", 1994),
        new("Inception", "Science Fiction", 2010),
        new("The Matrix", "Science Fiction", 1999),
        new("Fight Club", "Drama", 1999),
        new("The Lord of the Rings: The Return of the King", "Fantasy", 2003),
        new("Star Wars: Episode V - The Empire Strikes Back", "Science Fiction", 1980),
    ];
}
