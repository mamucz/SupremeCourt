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
        int Id{ get; }
        string Username { get; }
        string? ProfileImageUrlPath { get; }
        int numberOfLives { get; set; }
        bool IsEliminated { get; set; }
        public bool IsAi { get; }
    }
}
