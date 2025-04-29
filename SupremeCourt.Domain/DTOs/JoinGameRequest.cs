namespace SupremeCourt.Domain.DTOs
{
    public class JoinGameRequest
    {
        public Guid WaitingRoomId { get; set; }
        public int PlayerId { get; set; }
    }
}