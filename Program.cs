using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MovieAPI.Services;
using MovieApi.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMovieService, MovieService>();
builder.Services.AddSingleton<IMovieRepository, MovieRepository>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
