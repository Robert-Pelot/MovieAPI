using System.Collections.Generic;
using MovieApi.Models;
using MovieApi.Repository;

namespace MovieAPI.Services
{
    public class MovieService : IMovieService
    {
        private IMovieRepository _repo;

        public MovieService(IMovieRepository repo)
        {
            _repo = repo;            
        }

        public IEnumerable<Movie> GetMovies()
        {
            IEnumerable<Movie> myList = _repo.GetAll();
            return myList;
        }
        public Movie GetMovieByName(string name)
        {
            return _repo.GetMovieByName(name);
            // format movie and return
        }
        public IEnumerable<Movie> GetMoviesByYear(int year)
        {
            IEnumerable<Movie> mylist = _repo.GetAll();
            List<Movie> result = new List<Movie>();
            foreach (Movie m in mylist){
                if(m.Year == year)
                    result.Add(m);
            }
            return result;
        }
        public void CreateMovie(Movie m)
        {
            _repo.Insert(m);
        }
        public void UpdateMovie(string name, Movie m)
        {
            _repo.Update(name, m);
        }
        public void DeleteMovie(string name) 
        { _repo.Delete(name); }
    }
}