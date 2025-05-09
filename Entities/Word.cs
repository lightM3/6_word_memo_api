namespace WordMemoryApi.Entities
{
    public class Word
    {
        public int Id { get; set; }
        public string EngWord { get; set; } = null!;
        public string TrWord { get; set; } = null!;
        public string? SampleSentence { get; set; }
        public string? ImagePath { get; set; }
        public string? Category { get; set; }
        public string? AudioPath { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public ICollection<UserWord>? UserWords { get; set; }
    }
}
