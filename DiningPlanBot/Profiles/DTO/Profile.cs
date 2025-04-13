namespace DiningPlanBot.Profiles.DTO;

public class Profile
{
    public long ChatId { get; set; }
    public required string Url { get; set; }
    public required string Name { get; set; }
}