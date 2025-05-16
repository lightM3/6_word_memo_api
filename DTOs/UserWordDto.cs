using WordMemoryApi.DTOs;

public class UserWordDto
{
    public int Id { get; set; }
    public int RepetitionCount { get; set; }
    public DateTime? NextRepetitionDate { get; set; }
    public bool IsMastered { get; set; }

    public WordDto Word { get; set; } = null!;
}
