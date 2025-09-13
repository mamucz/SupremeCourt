using SupremeCourt.Domain.Interfaces;
using System.Reflection;

namespace SupremeCourt.Application.Services;

public class RegistrAIPlayersService
{
    private readonly IPlayerRepository _playerRepository;

    public RegistrAIPlayersService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task RegisterAiPlayersAsync(CancellationToken cancellationToken)
    {
        // ✅ Vynutíme načtení sestavení, aby bylo dostupné v AppDomain
        _ = typeof(AiPlayers.GptPlayer).Assembly;

        // 🔍 Najdeme všechny typy z namespace AiPlayers implementující IPlayer
        var aiPlayerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a =>
            {
                try { return a.GetExportedTypes(); }
                catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
            })
            .Where(t => t is not null
                        && t.IsClass
                        && !t.IsAbstract
                        && t.Namespace != null
                        && t.Namespace.Contains("AiPlayers")
                        && typeof(IPlayer).IsAssignableFrom(t))
            .ToList();

        if (!aiPlayerTypes.Any())
        {
            Console.WriteLine("⚠️ Nebyly nalezeny žádné AI třídy.");
            return;
        }

        foreach (var type in aiPlayerTypes)
        {
            var typeName = type.Name; // např. "GptAiPlayer"
            await _playerRepository.EnsureAiPlayerExistsAsync(typeName, cancellationToken);
        }
    }
}