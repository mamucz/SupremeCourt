using Riok.Mapperly.Abstractions;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Domain.Mappings;

[Mapper]
public partial class WaitingRoomMapper
{
    public static readonly WaitingRoomMapper Instance = new();

    [MapProperty(nameof(WaitingRoom.Id), nameof(WaitingRoomDto.WaitingRoomId))]
    protected partial WaitingRoomDto ToDtoInternal(WaitingRoom entity);

    public WaitingRoomDto ToDto(WaitingRoom entity)
    {
        var dto = ToDtoInternal(entity);
        dto.Players = entity.Players
            .Select(PlayerMapper.Instance.ToDto)
            .ToList();
        return dto;
    }

    [MapProperty(nameof(WaitingRoomDto.WaitingRoomId), nameof(WaitingRoom.Id))]
    public partial WaitingRoom ToEntity(WaitingRoomDto dto);

    [MapProperty(nameof(WaitingRoomSession.WaitingRoomId), nameof(WaitingRoomDto.WaitingRoomId))]
    [MapProperty(nameof(WaitingRoomSession.CreatedByPlayerId), nameof(WaitingRoomDto.CreatedByPlayerId))]
    [MapProperty(nameof(WaitingRoomSession.CreatedAt), nameof(WaitingRoomDto.CreatedAt))]
    [MapProperty(nameof(WaitingRoomSession.Players), nameof(WaitingRoomDto.Players))]
    public partial WaitingRoomDto ToDto(WaitingRoomSession session);

    [MapProperty(nameof(WaitingRoomSession.WaitingRoomId), nameof(WaitingRoom.Id))]
    [MapProperty(nameof(WaitingRoomSession.CreatedByPlayerId), nameof(WaitingRoom.CreatedByPlayerId))]
    [MapProperty(nameof(WaitingRoomSession.CreatedAt), nameof(WaitingRoom.CreatedAt))]
    [MapProperty(nameof(WaitingRoomSession.Players), nameof(WaitingRoom.Players))]
    public partial WaitingRoom ToEntity(WaitingRoomSession session);

    public WaitingRoomSession ToSession(WaitingRoom entity, int expirationSeconds = 180)
    {
        var session = new WaitingRoomSession();
        session.InitializeFromEntity(entity, expirationSeconds);
        return session;
    }
}