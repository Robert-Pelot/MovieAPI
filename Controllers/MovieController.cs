using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;
using MovieApi.Models;
using MovieAPI.Services;


namespace MovieAPIController.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly ILogger<MovieController> _logger;
    private IMovieService _service;
    public MovieController(ILogger<MovieController> logger, IMovieService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public IActionResult GetMovies()
    {
        IEnumerable<Movie> movies = _service.GetMovies();
        if (movies != null)
        {
            return Ok(movies);
        }
       else
        {
            return BadRequest("No movies available.");
        }
    }
    
    [HttpGet("{name}", Name="GetMovie")]
    public IActionResult GetMovieByName(string name)
    {
        Movie obj = _service.GetMovieByName(name);
        if(obj != null)
            return Ok(obj);
        else
            return BadRequest("Movie not found.");
    }


    [HttpGet("year/{year}")]
    public IActionResult GetMoviesByYear(int year)
    {
        var movies = _service.GetMoviesByYear(year);

        if (movies != null && movies.Any())  // check if collection has items
            return Ok(movies);
        else
            return NotFound("No movies found for the specified year.");
    }

    [HttpPost]
    public IActionResult CreateMovie(Movie m)
    {
        _service.CreateMovie(m);
        // add code to test to ensure it added successfully, if not return 500 error
        return CreatedAtRoute("GetMovie", new { name = m.Name }, m);
    }

    [HttpPut("{name}")]
    public IActionResult UpdateMovie(string name, Movie m)
    {
        _service.UpdateMovie(name, m);
        // add code to test to ensure it updated successfully, if not return 500 error
        return Ok("Movie updated successfully.");
    }

    [HttpDelete("{name}")]
    public IActionResult DeleteMovie(string name)
    {
        _service.DeleteMovie(name);
        // add code to test to ensure it deleted successfully, if not return 500 error
        return Ok("Movie deleted successfully.");
    }
}
