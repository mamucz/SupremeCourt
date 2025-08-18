using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SupremeCourt.Domain.Interfaces;

public sealed class AiPlayerFactory : IAiPlayerFactory
{
    private readonly IServiceProvider _services;
    private readonly IPlayerRepository _playerRepository;

    // lazy cache všech typů IAiPlayer (case-insensitive podle názvu třídy)
    private static readonly Lazy<IReadOnlyDictionary<string, Type>> _aiTypes =
        new Lazy<IReadOnlyDictionary<string, Type>>(DiscoverAiTypes, isThreadSafe: true);

    public AiPlayerFactory(IServiceProvider services, IPlayerRepository playerRepository)
    {
        _services = services;
        _playerRepository = playerRepository;
    }

    public Task<List<string>> GetAiPlayerTypesAsync()
    {
        var names = _aiTypes.Value.Keys
            .OrderBy(n => n)
            .ToList();

        return Task.FromResult(names);
    }

    public async Task<IAiPlayer> CreateAsync(string type)
    {
        if (!_aiTypes.Value.TryGetValue(type, out var implType))
        {
            var available = string.Join(", ", _aiTypes.Value.Keys.OrderBy(x => x));
            throw new ArgumentException(
                $"Unknown AI player type '{type}'. Available: {available}", nameof(type));
        }

        // 1) zajisti, že pro daný typ existuje DB hráč
        await _playerRepository.EnsureAiPlayerExistsAsync(type, CancellationToken.None);

        // 2) vytvoř instanci typu IAiPlayer s DI (AI třídy mohou mít své služby v ctoru)
        var instance = (IAiPlayer)ActivatorUtilities.CreateInstance(_services, implType);
        return instance;
    }

    private static IReadOnlyDictionary<string, Type> DiscoverAiTypes()
    {
        var iface = typeof(IAiPlayer);

        // vynecháme dynamická assembly; procházíme exportované public typy
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();

        var types = assemblies
            .SelectMany(a =>
            {
                try { return a.GetExportedTypes(); }
                catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
            })
            .Where(t => t is not null
                        && !t!.IsAbstract
                        && iface.IsAssignableFrom(t!)
                        && t!.IsClass)
            .ToDictionary(t => t!.Name, t => t!, StringComparer.OrdinalIgnoreCase);

        return types;
    }
}
