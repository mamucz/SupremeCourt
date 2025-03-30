namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class CreatePlayerCommand
    {
        public int UserId { get; set; } // Použití UserId místo Name
    }
}