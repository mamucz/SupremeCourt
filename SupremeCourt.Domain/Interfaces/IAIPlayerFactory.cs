using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

public interface IAiPlayerFactory
{
    Task<List<string>> GetAiPlayerTypesAsync();
    Task<IAiPlayer> CreateAsync(string type); // vrací přímo entitu Player
}