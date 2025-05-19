using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{
    public class AddAiPlayersBulkDto
    {
        public int Count { get; set; }
        public string Type { get; set; } = "Random";
    }
}
