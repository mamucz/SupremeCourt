using Riok.Mapperly.Abstractions;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Domain.Mappings
{
    [Mapper]
    public partial class WaitingRoomMapper
    {
        // Mapování entita -> DTO
        [MapProperty(nameof(WaitingRoom.Id), nameof(WaitingRoomDto.WaitingRoomId))]
        [MapperIgnoreSource(nameof(WaitingRoom.GameId))]
        [MapperIgnoreSource(nameof(WaitingRoom.Game))]     
        public partial WaitingRoomDto ToDto(WaitingRoom entity);
        // Mapování DTO -> entita
        public partial WaitingRoom ToEntity(WaitingRoomDto dto);
        // Mapování entita -> session model
        public partial WaitingRoomSession ToSession(WaitingRoom entity);
        public partial WaitingRoomDto ToDto(WaitingRoomSession session);
    }
}
