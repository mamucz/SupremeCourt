using Riok.Mapperly.Abstractions;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Domain.Mappings;

[Mapper]
public partial class WaitingRoomMapper
{
    public static readonly WaitingRoomMapper Instance = new();

    // 🧭 Entita -> DTO
    [MapProperty(nameof(WaitingRoom.Id), nameof(WaitingRoomDto.WaitingRoomId))]
    public partial WaitingRoomDto ToDto(WaitingRoom entity);

    // 🧭 DTO -> Entita
    [MapProperty(nameof(WaitingRoomDto.WaitingRoomId), nameof(WaitingRoom.Id))]
    public partial WaitingRoom ToEntity(WaitingRoomDto dto);

    // 🧭 Entita -> Session (např. při spuštění místnosti)
    public partial WaitingRoomSession ToSession(WaitingRoom entity);

    // 🧭 Session -> DTO (pro zobrazení ve frontendu)
    [MapProperty(nameof(WaitingRoomSession.WaitingRoomId), nameof(WaitingRoomDto.WaitingRoomId))]
    [MapProperty(nameof(WaitingRoomSession.CreatedByPlayerId), nameof(WaitingRoomDto.CreatedByPlayerId))]
    [MapProperty(nameof(WaitingRoomSession.CreatedAt), nameof(WaitingRoomDto.CreatedAt))]
    [MapProperty(nameof(WaitingRoomSession.Players), nameof(WaitingRoomDto.Players))]
    public partial WaitingRoomDto ToDto(WaitingRoomSession session);

    // 🧭 Session -> Entita (např. pro uložení zpět do DB, pokud potřebuješ)
    [MapProperty(nameof(WaitingRoomSession.WaitingRoomId), nameof(WaitingRoom.Id))]
    [MapProperty(nameof(WaitingRoomSession.CreatedByPlayerId), nameof(WaitingRoom.CreatedByPlayerId))]
    [MapProperty(nameof(WaitingRoomSession.CreatedAt), nameof(WaitingRoom.CreatedAt))]
    [MapProperty(nameof(WaitingRoomSession.Players), nameof(WaitingRoom.Players))]
    public partial WaitingRoom ToEntity(WaitingRoomSession session);
}
