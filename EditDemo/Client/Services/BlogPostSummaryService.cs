using Models;
using Newtonsoft.Json;

namespace Client.Services;

public class BlogPostSummaryService
{
    private readonly HttpClient http;

    public IEnumerable<BlogPost>? Summaries;

    public BlogPostSummaryService(HttpClient http)
    {
        this.http = http ??
            throw new ArgumentNullException(nameof(http));
    }

    public async Task LoadBlogPostSummaries()
    {
        if (Summaries == null)
        {
            var result = await http.GetStringAsync("api/blogposts ");
            Summaries = JsonConvert.DeserializeObject<BlogPost[]>(result);
        }
    }

}
