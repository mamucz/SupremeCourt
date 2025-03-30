using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{
    public class PlayerDto
    {
        public int PlayerId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
