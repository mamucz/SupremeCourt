using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

public interface IAIPlayerFactory
{
    Task<List<string>> GetAiPlayerTypesAsync();
    Task<IAiPlayer> CreateAsync(string type); // vrací přímo entitu Player
}