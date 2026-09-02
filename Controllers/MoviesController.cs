using Microsoft.AspNetCore.Mvc;
using MovieApi.Dtos;
using MovieApi.Repositories;
using MovieApi.Services;

namespace MovieApi.Controllers;

[ApiController]
[Route("api/movies")]
public sealed class MoviesController(IMovieService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<MovieResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<MovieResponse>> GetMovies([FromQuery] int? year = null) =>
        Ok(service.GetMovies(year));

    [HttpGet("{name}", Name = nameof(GetMovie))]
    [ProducesResponseType<MovieResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MovieResponse> GetMovie(string name)
    {
        var movie = service.GetMovie(name);
        return movie is null ? NotFound() : Ok(movie);
    }

    [HttpPost]
    [ProducesResponseType<MovieResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<MovieResponse> CreateMovie(MovieRequest request)
    {
        if (!service.CreateMovie(request, out var movie))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "A movie with that name already exists.",
            });
        }

        return CreatedAtRoute(nameof(GetMovie), new { name = movie.Name }, movie);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult UpdateMovie(string name, MovieRequest request) => service.UpdateMovie(name, request) switch
    {
        UpdateResult.Updated => NoContent(),
        UpdateResult.NotFound => NotFound(),
        UpdateResult.DuplicateName => Conflict(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "A movie with that name already exists.",
        }),
        _ => throw new InvalidOperationException("Unexpected update result."),
    };

    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteMovie(string name) =>
        service.DeleteMovie(name) ? NoContent() : NotFound();
}
