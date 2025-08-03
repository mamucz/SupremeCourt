using SupremeCourt.Domain.Interfaces;
using System.Reflection;

namespace SupremeCourt.Application.Services
{
    public class RegistrAIPlayersService
    {
        private readonly IPlayerRepository _playerRepository;

        public RegistrAIPlayersService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task RegisterAiPlayersAsync(CancellationToken cancellationToken)
        {
            // Najdi všechny typy, které implementují IAIPlayer
            var aiPlayerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.FullName!.Contains("AIPlayers")) // jen knihovny obsahující "AIPlayers"
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(t => typeof(IPlayer).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .ToList();

            if (!aiPlayerTypes.Any())
                return;

            foreach (var type in aiPlayerTypes)
            {
                // Jméno typu např. "GptAiPlayer"
                var typeName = type.Name;

                await _playerRepository.EnsureAiPlayerExistsAsync(typeName, cancellationToken);
            }
        }
    }
}