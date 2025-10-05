using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class CreatePlayerCommand
    {
        public User User { get; set; } // Použití UserId místo Name
    }
}