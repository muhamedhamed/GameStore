using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        //GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
        await dbContext.Games
                .Include(game => game.Genre)
                .Select(game => game.ToGameSummaryDto())
                .AsNoTracking()
                .ToListAsync());

        //GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            // GameDto? game = games.Find(x => x.Id == id);

            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());

        })
        .WithName(GetGameEndpointName);

        //POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            //Instead of the mapping on the current class we will do external mapping
            Game game = newGame.ToEntity();
            // game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        });

        //PUT /games
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGameDto, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(updateGameDto.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games.Where(game => game.Id == id)
            .ExecuteDeleteAsync();
            // games.RemoveAll(x => x.Id == id);

            return Results.NoContent();
        });

        return group;
    }
}

