using System.Collections.Generic;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// var connectionString ="Data Source =GameStore.db";
var connectionString = builder.Configuration.GetConnectionString("GameStore");

builder.Services.AddSqlite<GameStoreContext>(connectionString);

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

await app.MigrateDbAsync();

app.Run();
