using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{
    public class ActualPlayerStateDto
    {
        /// <summary>
        /// Is player logged in?
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// ID místnosti, pokud je hráč ve Waiting Room. Jinak null.
        /// </summary>
        public Guid? IsInWaitingRoom { get; set; }

        /// <summary>
        /// ID hry, pokud je hráč ve hře. Jinak null.
        /// </summary>
        public int? IsInGame { get; set; }
    }

}
