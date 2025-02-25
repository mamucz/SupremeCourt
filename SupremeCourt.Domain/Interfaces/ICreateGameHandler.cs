using System.Threading.Tasks;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Domain.Interfaces
{
    public interface ICreateGameHandler
    {
        Task<Game?> HandleAsync(CreateGameDto dto); // ✅ Upraveno na `HandleAsync`
    }
}