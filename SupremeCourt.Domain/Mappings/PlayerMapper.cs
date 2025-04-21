using Riok.Mapperly.Abstractions;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Mappings;

namespace SupremeCourt.Domain.Mappings;
[Mapper]
public partial class PlayerMapper
{
    public static readonly PlayerMapper Instance = new();
    // ✅ Základní mapování pomocí Mapperly
    private partial PlayerDto ToDtoInternal(Player player);

    // ✅ Veřejná metoda doplňující URL po mapování
    public PlayerDto ToDto(Player player)
    {
        var dto = ToDtoInternal(player);
        dto.ProfileImageUrl = $"/api/player/{player.Id}/profile-picture";
        return dto;
    }
}