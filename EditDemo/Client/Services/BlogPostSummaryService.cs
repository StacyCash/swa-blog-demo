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

    public async void Add(BlogPost blogPost)
    {
        if (blogPost == null)
        {
            throw new ArgumentNullException(nameof(blogPost));
        }

        if (Summaries is null)
        {
            Summaries = new List<BlogPost>();
        }

        if (blogPost.BlogPostMarkdown?.Length > 250)
        {
            blogPost.BlogPostMarkdown = blogPost.BlogPostMarkdown.Substring(0, 250);
        }

        var list = Summaries.ToList();
        list.Add(blogPost);
        Summaries = list;
    }

    public void Replace(BlogPost blogPost)
    {
        if (Summaries == null || !Summaries.Any(bp => bp.Id == blogPost.Id))
        {
            return;
        }

        var list = Summaries.ToList();
        _ = list.Remove(list.FirstOrDefault(bp => bp.Id == blogPost.Id)!);
        list.Add(blogPost);
        Summaries = list;
    }
}
