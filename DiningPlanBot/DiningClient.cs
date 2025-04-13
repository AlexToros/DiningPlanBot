namespace DiningPlanBot;

public class DiningClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DiningParser _parser;

    public DiningClient(IHttpClientFactory httpClientFactory, DiningParser parser)
    {
        _httpClientFactory = httpClientFactory;
        _parser = parser;
    }

    public async Task<DiningPlan> GetDiningPlanAsync(string diningPlanUrl)
    {
        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(diningPlanUrl);
        
        var response = await client.GetAsync("");

        var rawHttp = await response.Content.ReadAsStringAsync();
        
        return _parser.Parse(rawHttp, diningPlanUrl);
    }
}