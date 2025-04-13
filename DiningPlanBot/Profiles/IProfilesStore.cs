namespace DiningPlanBot.Profiles;

public interface IProfilesStore
{
    Task SaveProfileAsync(long chatIt, DiningPlan diningPlan);
}