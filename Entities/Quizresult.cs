namespace WordMemoryApi.Entities
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CorrectCount { get; set; }
        public int TotalCount { get; set; }
        public DateTime QuizDate { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}
