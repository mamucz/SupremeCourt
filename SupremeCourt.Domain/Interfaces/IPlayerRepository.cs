using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces;

/// <summary>
/// Rozhraní pro správu hráčů v paměti.
/// </summary>
public interface IPlayerRepository
{

    Player? GetById(Guid id);


    Task<List<Player>> GetAll();

    Player CreatePlayer(User user);

    void Delete(Player player);

    List<Player> GetAllAiPlayers(CancellationToken cancellationToken);

}