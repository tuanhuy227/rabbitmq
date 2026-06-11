using System.ComponentModel.DataAnnotations;

namespace SendMailApi.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }
}
