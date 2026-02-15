
namespace MovieApi.Models {
    public class Movie {
        public string Name {get; set;}
        public string Genre {get; set;}
        public int Year {get; set;}
        public override string ToString() {
        return $"{Name}, {Genre}, {Year}";
        }
    }
}