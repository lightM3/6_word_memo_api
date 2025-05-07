namespace WordMemoryApi.Entities
{
    public class UserWord
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int WordId { get; set; }
        public Word Word { get; set; } = null!;

        public int RepetitionCount { get; set; } = 0;
        public DateTime? NextRepetitionDate { get; set; }

        public bool IsMastered { get; set; } = false;
    }
}
