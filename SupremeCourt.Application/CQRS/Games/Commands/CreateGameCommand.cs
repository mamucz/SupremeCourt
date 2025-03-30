using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.CQRS.Games.Commands
{
    public class CreateGameCommand
    {
        public CreateGameDto Dto { get; }

        public CreateGameCommand(CreateGameDto dto)
        {
            Dto = dto;
        }
    }
}