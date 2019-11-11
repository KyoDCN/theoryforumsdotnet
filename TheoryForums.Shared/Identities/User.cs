using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TheoryForums.Shared.Models
{
    public class User : IdentityUser<int>
    {
        public string DisplayName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime JoinDate { get; set; }
        public string AvatarUrl { get; set; } = "https://png.pngtree.com/svg/20161027/service_default_avatar_182956.png";
    }
}
