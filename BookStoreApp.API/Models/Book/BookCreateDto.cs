using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Book
{
    public class BookCreateDto
    {
        [Required]
        public string? Title { get; set; }
        [Range(0,int.MaxValue)]
        public int Year { get; set; }
        public string? Isbn { get; set; }
        [StringLength(250, MinimumLength = 10)]
        public string? Summary { get; set; }
        public string? ImageData { get; set; }
        public string? OriginalImageName { get; set; }
        [Required]
        [Range(0,int.MaxValue)]
        public decimal Price { get; set; }

        public int? AuthorId { get; set; }
    }
}
