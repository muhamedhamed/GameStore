﻿namespace GameStore.Api.Entities;

public class Genre
{
    public int Id { get; set; }
    // public string Name { get; set; }=string.Empty;
    // public string? Name { get; set; };
    public required string Name { get; set; }
}
