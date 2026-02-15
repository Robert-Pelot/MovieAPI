using System.Collections.Generic;
using MovieApi.Models;


namespace MovieApi.Repository
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAll();
        public Movie GetMovieByName(string name);
        void Insert(Movie m);
        void Update(string name, Movie m);
        void Delete(string name);
    }
}