using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using System.Collections.Concurrent;

namespace SupremeCourt.Infrastructure.Repositories;

/// <summary>
/// In-memory implementace repository pro hráče (Player).
/// Nepoužívá databázi – veškeré operace probíhají v paměti.
/// </summary>
public class PlayerRepository : IPlayerRepository
{
    private readonly ConcurrentDictionary<Guid, Player> _players = new();
    
    public Player? GetById(Guid id)
    {
        _players.TryGetValue(id, out var player);
        return player;
    }

    public Task<List<Player>> GetAll()
    {
        return Task.FromResult(_players.Values.ToList());
    }

    public Player CreatePlayer(User user)
    {
        Player newPalyer = new Player(user);
        newPalyer.User = user;
        newPalyer.Id = Guid.NewGuid();

        _players.TryAdd(newPalyer.Id, newPalyer);
        return newPalyer;
    }

    public void Delete(Player player)
    {
        _players.TryRemove(player.Id, out _);
        return ;
    }

    public List<Player> GetAllAiPlayers(CancellationToken cancellationToken)
    {
        var aiPlayers = _players.Values
            .Where(p => p.IsAi)
            .ToList();

        return aiPlayers;
    }
}
