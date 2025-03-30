namespace SupremeCourt.Domain.DTOs
{
    public class JoinGameRequest
    {
        public int WaitingRoomId { get; set; }
        public int PlayerId { get; set; }
    }
}