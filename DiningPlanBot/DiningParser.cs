using HtmlAgilityPack;

namespace DiningPlanBot;

public class DiningParser
{
    public DiningPlan Parse(string rawHttp, string url)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(rawHttp);

        var result = new DiningPlan
        {
            Url = url,
            Pupil = doc.DocumentNode.SelectNodes("//h2[contains(@class, 'POTREB')]").First().InnerText,
        };
        
        
        
        return result;
    }
}