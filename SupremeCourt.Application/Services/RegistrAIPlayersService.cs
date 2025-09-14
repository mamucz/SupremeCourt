using SupremeCourt.Domain.Interfaces;
using System.Reflection;

namespace SupremeCourt.Application.Services;

/// <summary>
/// Služba pro registraci všech dostupných AI hráčů do databáze.
/// 
/// Prochází všechny načtené .NET assembly a hledá typy implementující <see cref="IPlayer"/> v namespace `AiPlayers`.
/// Pokud typ ještě není v databázi, zavolá <see cref="IPlayerRepository.EnsureAiPlayerExistsAsync"/>.
/// </summary>
public class RegistrAIPlayersService
{
    private readonly IPlayerRepository _playerRepository;

    /// <summary>
    /// Inicializuje novou instanci <see cref="RegistrAIPlayersService"/>.
    /// </summary>
    /// <param name="playerRepository">Repozitář hráčů, který umožňuje registraci AI typů do databáze.</param>
    public RegistrAIPlayersService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    /// <summary>
    /// Načte všechny typy AI hráčů v namespace `AiPlayers`, které implementují <see cref="IPlayer"/>,
    /// a zaregistruje je do databáze, pokud ještě neexistují.
    /// </summary>
    /// <param name="cancellationToken">Token pro zrušení operace.</param>
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
