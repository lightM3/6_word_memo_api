namespace WordMemoryApi.DTOs
{
    public class WordDto
    {
        public string EngWord { get; set; } = null!;
        public string TrWord { get; set; } = null!;
        public string? SampleSentence { get; set; }
        public string? ImagePath { get; set; }
        public string? Category { get; set; }
    }
}
