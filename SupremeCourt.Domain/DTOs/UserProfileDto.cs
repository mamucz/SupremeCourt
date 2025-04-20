using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{

    public class UserProfileDto
    {
        public string UserName { get; set; } = string.Empty;
        public byte[]? ProfileImage { get; set; }

        public string? ProfileImageMimeType { get; set; } 
        public string PasswordHash { get; set; }
    }
}
    
