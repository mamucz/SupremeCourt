using Riok.Mapperly.Abstractions;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Mappings;

namespace SupremeCourt.Domain.Mappings;
[Mapper]
public partial class PlayerMapper
{
    public static readonly PlayerMapper Instance = new();

    [MapProperty(nameof(Player.Id), nameof(PlayerDto.PlayerId))]
    [MapProperty(nameof(Player.User.Username), nameof(PlayerDto.Username))]
    public partial PlayerDto ToDtoInternal(Player player); // 👈 změněno z private na public

    public PlayerDto ToDto(Player player)
    {
        var dto = ToDtoInternal(player);
        dto.ProfileImageUrl = $"/api/player/{player.Id}/profile-picture";
        return dto;
    }
}