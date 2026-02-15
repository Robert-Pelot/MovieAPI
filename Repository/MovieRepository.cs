using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using MovieApi.Models;



namespace MovieApi.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private static readonly List<Movie> movies = new List<Movie>(10)
        {
            new Movie { Name = "The Shawshank Redemption", Genre = "Drama", Year = 1994 },
            new Movie { Name = "The Godfather", Genre = "Crime", Year = 1972 },
            new Movie { Name = "The Dark Knight", Genre = "Action", Year = 2008 },
            new Movie { Name = "Pulp Fiction", Genre = "Crime", Year = 1994 },
            new Movie { Name = "Forrest Gump", Genre = "Drama", Year = 1994 },
            new Movie { Name = "Inception", Genre = "Sci-Fi", Year = 2010 },
            new Movie { Name = "The Matrix", Genre = "Sci-Fi", Year = 1999 },
            new Movie { Name = "Fight Club", Genre = "Drama", Year = 1999 },
            new Movie { Name = "The Lord of the Rings: The Return of the King", Genre = "Fantasy", Year = 2003 },
            new Movie { Name = "Star Wars: Episode V - The Empire Strikes Back", Genre = "Sci-Fi", Year = 1980 }

        };

        public MovieRepository()
        {
        }

        public IEnumerable<Movie> GetAll()
        {
            return movies;
        }

        public Movie GetMovieByName(string name)
        {
            foreach(Movie m in movies){
            if(m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return m;
            }
            return null;
        }

        public void Insert(Movie m)
        {
            movies.Add(m);
        }

        public void Update(string name,Movie movieIn)
        {
            foreach(Movie m in movies){
                if(m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)){
                    m.Name = movieIn.Name;
                    m.Genre = movieIn.Genre;
                    m.Year = movieIn.Year;
                }
            }
        }

        public void Delete(string name)
        {
            for (int i = movies.Count - 1; i >= 0; i--)
            {
                if (movies[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    movies.RemoveAt(i);
                }
            }
        }
    }
}

