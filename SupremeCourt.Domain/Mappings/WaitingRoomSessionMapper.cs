using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Domain.Mappings
{
    public static class WaitingRoomSessionMapper
    {
        public static WaitingRoomDto ToDto(WaitingRoomSession session)
        {
            return new WaitingRoomDto
            {
                WaitingRoomId = session.WaitingRoomId,
                CreatedAt = session.CreatedAt,
                CreatedByPlayerId = session.CreatedBy.Id,
                CreatedByPlayerName = session.CreatedBy.Username,
                TimeLeftSeconds = session.GetTimeLeft(),
                Players = session.Players.Select(ToPlayerDto).ToList()
            };
        }

        private static PlayerDto ToPlayerDto(IPlayer player)
        {
            return new PlayerDto
            {
                PlayerId = player.Id,
                Username = player.Username
            };
        }
    }
}