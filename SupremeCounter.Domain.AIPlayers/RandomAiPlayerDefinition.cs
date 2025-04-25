using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Domain.AIPlayers
{
    public class RandomAiPlayerDefinition : IAIPlayerDefinition
    {
        public string Username => "AI-Random";
        public string BotType => "Random";
        public string? ProfilePicturePath => "assets/img/ai-avatar.png";
    }
}
