namespace DexefTask.BusinessLogic.DTO.Book
{
    public class BookToReturnDTO
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public int PublishedYear { get; set; }
        public bool IsAvailable { get; set; }
    }
}
