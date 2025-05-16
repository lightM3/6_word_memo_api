using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;
using WordMemoryApi.Entities;

public class UserSettings
{
    public int Id { get; set; }

    public int DailyWordLimit { get; set; } = 3;

    public DateTime LastWordAdditionDate { get; set; } = DateTime.MinValue;

    public int UserId { get; set; }

    [JsonIgnore] 
    [BindNever]  
    public User User { get; set; } = null!;
}
