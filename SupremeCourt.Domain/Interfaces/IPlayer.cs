using SupremeCourt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IPlayer
    {
        Guid Id{ get; }
        string Username { get; }
        string? ProfileImageUrlPath { get; }
        int NumberOfLives { get; set; }
        bool IsEliminated { get; set; }
        bool IsAi { get; }

        Guid ActiveWaitingRoom { get; set; }
        Guid ActiveGame { get; set; }
    }
}
