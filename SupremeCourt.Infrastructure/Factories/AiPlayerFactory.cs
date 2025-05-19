using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

public class AiPlayerFactory : IAIPlayerFactory
{
    private readonly IPlayerRepository _playerRepository;

    public static Dictionary<string, Type> AiPlayerTypes =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IAiPlayer).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name.Replace("AiPlayer", ""), t => t, StringComparer.OrdinalIgnoreCase);

    public AiPlayerFactory(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<List<string>> GetAiPlayerTypesAsync()
    {
        var aiPlayerTypes = AiPlayerTypes.Keys.ToList();
        return await Task.FromResult(aiPlayerTypes);
    }   
    public async Task<IAiPlayer> CreateAsync(string type)
    {
        if (!typeof(IAiPlayer).IsAssignableFrom(AiPlayerTypes[type]))
            throw new ArgumentException("Zadaný typ není AI hráč", nameof(type));

        if (Activator.CreateInstance(AiPlayerTypes[type]) is not IAiPlayer aiInstance)
            throw new Exception($"Nepodařilo se vytvořit instanci AI hráče z typu {type.Name}");

        return await _playerRepository.EnsureAiPlayerExistsAsync(aiInstance.Username);
    }
}