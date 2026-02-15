using System.Collections.Generic;
using MovieApi.Models;


namespace MovieAPI.Services
{
    public interface IMovieService
    {
        IEnumerable<Movie> GetMovies();
        public Movie GetMovieByName(string name);
        public IEnumerable<Movie> GetMoviesByYear(int year);
        public void CreateMovie(Movie m);
        public void UpdateMovie(string name, Movie m);
        public void DeleteMovie(string name);
    }
}
