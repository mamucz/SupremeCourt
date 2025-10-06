using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.DTOs
{
    public class JoinGameRequest
    {
        public Guid WaitingRoomId { get; set; }
        public Player Player { get; set; }
    }
}