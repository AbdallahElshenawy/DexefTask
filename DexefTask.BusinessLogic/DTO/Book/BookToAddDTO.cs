using System.ComponentModel.DataAnnotations;

namespace DexefTask.BusinessLogic.DTO.Book
{
    public class BookToAddDTO
    {
        [Required]
        [MaxLength(30)]
        public string? Title { get; set; }
        
        [Required]
        [MaxLength(30)]
        public string? Author { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Genre { get; set; }
        public int PublishedYear { get; set; }
        public bool IsAvailable { get; set; }

    }
}
