using System.ComponentModel.DataAnnotations;
namespace BookStoreApp.API.Models.Author
{
    public class AuthorUpdateDto: BaseDto
    {

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }
        [StringLength(500)]
        public string? Bio { get; set; }
    }
}
