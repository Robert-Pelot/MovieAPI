using System.Net;
using System.Net.Http.Json;
using MovieApi.Dtos;
using Xunit;

namespace MovieApi.Tests;

public sealed class MoviesApiTests : IClassFixture<MovieApiFactory>
{
    private readonly HttpClient _client;

    public MoviesApiTests(MovieApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMovies_ReturnsSeededMovies()
    {
        var response = await _client.GetAsync("/api/movies");
        var movies = await response.Content.ReadFromJsonAsync<MovieResponse[]>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(movies);
        Assert.NotEmpty(movies);
    }

    [Fact]
    public async Task GetMovies_WithYearFilter_ReturnsOnlyMatchingMovies()
    {
        var movies = await _client.GetFromJsonAsync<MovieResponse[]>("/api/movies?year=1972");

        var movie = Assert.Single(Assert.IsType<MovieResponse[]>(movies));
        Assert.Equal("The Godfather", movie.Name);
    }

    [Fact]
    public async Task GetMovie_IsCaseInsensitive()
    {
        var response = await _client.GetAsync("/api/movies/the%20godfather");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMovie_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/movies/Unknown");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateMovie_ReturnsCreatedMovieAndLocation()
    {
        var request = UniqueMovie();
        var response = await _client.PostAsJsonAsync("/api/movies", request);
        var movie = await response.Content.ReadFromJsonAsync<MovieResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(request.Name, movie?.Name);
        Assert.Contains(Uri.EscapeDataString(request.Name), response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task CreateMovie_TrimsTextValues()
    {
        var name = $"Trimmed {Guid.NewGuid():N}";
        var response = await _client.PostAsJsonAsync("/api/movies", new MovieRequest
        {
            Name = $"  {name}  ", Genre = "  Drama  ", Year = 2020,
        });
        var movie = await response.Content.ReadFromJsonAsync<MovieResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(name, movie?.Name);
        Assert.Equal("Drama", movie?.Genre);
    }

    [Fact]
    public async Task CreateMovie_WhenNameAlreadyExists_ReturnsConflict()
    {
        var response = await _client.PostAsJsonAsync("/api/movies", new MovieRequest
        {
            Name = "the godfather", Genre = "Crime", Year = 1972,
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [InlineData("", "Drama", 2000)]
    [InlineData("   ", "Drama", 2000)]
    [InlineData("Valid name", "   ", 2000)]
    [InlineData("Valid name", "Drama", 1800)]
    [InlineData("Valid name", "Drama", 2200)]
    public async Task CreateMovie_WithInvalidInput_ReturnsBadRequest(string name, string genre, int year)
    {
        var response = await _client.PostAsJsonAsync("/api/movies", new MovieRequest
        {
            Name = name, Genre = genre, Year = year,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_UpdatesAndCanRenameMovie()
    {
        var original = UniqueMovie();
        await _client.PostAsJsonAsync("/api/movies", original);
        var renamed = new MovieRequest
        {
            Name = $"Renamed {Guid.NewGuid():N}", Genre = "Comedy", Year = 2021,
        };

        var response = await _client.PutAsJsonAsync($"/api/movies/{Uri.EscapeDataString(original.Name)}", renamed);
        var updated = await _client.GetFromJsonAsync<MovieResponse>($"/api/movies/{Uri.EscapeDataString(renamed.Name)}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(updated);
        Assert.Equal(renamed.Name, updated.Name);
        Assert.Equal(renamed.Genre, updated.Genre);
        Assert.Equal(renamed.Year, updated.Year);
    }

    [Fact]
    public async Task UpdateMovie_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/movies/Unknown", UniqueMovie());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_WhenNewNameExists_ReturnsConflict()
    {
        var response = await _client.PutAsJsonAsync("/api/movies/Inception", new MovieRequest
        {
            Name = "The Matrix", Genre = "Science Fiction", Year = 1999,
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_WithInvalidInput_ReturnsBadRequest()
    {
        var response = await _client.PutAsJsonAsync("/api/movies/Inception", new MovieRequest
        {
            Name = " ", Genre = "Science Fiction", Year = 2010,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_RemovesMovie()
    {
        var movie = UniqueMovie();
        await _client.PostAsJsonAsync("/api/movies", movie);

        var response = await _client.DeleteAsync($"/api/movies/{Uri.EscapeDataString(movie.Name)}");
        var getResponse = await _client.GetAsync($"/api/movies/{Uri.EscapeDataString(movie.Name)}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/movies/Unknown");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static MovieRequest UniqueMovie() => new()
    {
        Name = $"Test Movie {Guid.NewGuid():N}", Genre = "Drama", Year = 2024,
    };
}
