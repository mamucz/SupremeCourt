using Riok.Mapperly.Abstractions;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Mappings;

[Mapper]
public partial class PlayerMapper
{
    public static readonly PlayerMapper Instance = new();

    [MapProperty(nameof(Player.Id), nameof(PlayerDto.PlayerId))]
    [MapProperty(nameof(Player.User.Username), nameof(PlayerDto.Username))]
    public partial PlayerDto ToDto(Player player);
}