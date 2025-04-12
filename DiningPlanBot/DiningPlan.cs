namespace DiningPlanBot;

public class DiningPlan
{
    public required string Pupil { get; set; }
    public required string Url { get; set; }
    public DaylyPlan[] Days { get; set; } = null!;
}

public class DaylyPlan
{
    public required string DayName { get; set; }
    public Dish[] Breakfast { get; set; } = null!;
    public Dish[] Lunch { get; set; } = null!;
    public Dish[] Snacks { get; set; } = null!;
}

public class Dish
{
    public required string Name { get; set; }
    public required string Id { get; set; }
    public int Count { get; set; }
}