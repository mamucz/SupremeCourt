using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.Games.Commands
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