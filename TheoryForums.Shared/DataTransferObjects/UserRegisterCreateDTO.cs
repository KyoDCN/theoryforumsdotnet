using System.ComponentModel.DataAnnotations;
using TheoryForums.Shared.Validators;

namespace TheoryForums.Shared.DataTransferObjects
{
    public class UserRegisterCreateDTO
    {
        [Required]
        // [MinLength(6, ErrorMessage = "Must be at least 6 characters long")]
        [RegularExpression(@"^[\w]*[^_\W]", ErrorMessage = "Can only contain letters and numbers")]
        public string UserName { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Must be at least 3 characters long"), MaxLength(26, ErrorMessage = "Must be less than 26 characters long")]
        [RegularExpression(@"^[\w ]*[^_\W]", ErrorMessage = "Can only contain letters, numbers, and spaces")]
        [StringEqual(nameof(UserName), ErrorMessage = "Display Name and Username cannot be the same.")]
        public string DisplayName { get; set; }
        
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
