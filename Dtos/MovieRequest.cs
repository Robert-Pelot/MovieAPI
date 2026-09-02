using System.ComponentModel.DataAnnotations;

namespace MovieApi.Dtos;

public sealed class MovieRequest : IValidatableObject
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Genre { get; init; } = string.Empty;

    [Range(1888, 2100)]
    public int Year { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult("Name cannot be blank.", [nameof(Name)]);
        }

        if (string.IsNullOrWhiteSpace(Genre))
        {
            yield return new ValidationResult("Genre cannot be blank.", [nameof(Genre)]);
        }
    }
}
