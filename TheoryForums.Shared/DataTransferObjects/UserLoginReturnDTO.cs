using System;
using System.Collections.Generic;
using System.Text;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class UserLoginReturnDTO
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime JoinDate { get; set; }
        public string Email { get; set; }
    }
}
