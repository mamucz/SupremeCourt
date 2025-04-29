using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{
    public class WaitingRoomCreatedDto
    {
        public Guid WaitingRoomId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByPlayerId { get; set; }
    }
}
