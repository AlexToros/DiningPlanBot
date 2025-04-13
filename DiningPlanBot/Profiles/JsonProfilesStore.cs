using System.Text.Json;
using DiningPlanBot.Profiles.DTO;
using Microsoft.Extensions.Options;

namespace DiningPlanBot.Profiles;

public class JsonProfilesStore : IProfilesStore
{
    private readonly IOptions<AppSettings> _appSettings;

    public JsonProfilesStore(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }
    public async Task SaveProfileAsync(long chatIt, DiningPlan diningPlan)
    {
        var profile = new Profile
        {
            Name = diningPlan.Pupil,
            ChatId = chatIt,
            Url = diningPlan.Url
        };
        var fileName = Path.Combine(_appSettings.Value.PathToProfiles, $"{chatIt}.json");
        await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(profile));
    }
}