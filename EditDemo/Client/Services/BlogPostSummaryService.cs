using Models;
using Newtonsoft.Json;

namespace Client.Services;

public class BlogPostSummaryService
{
    private readonly HttpClient http;

    public List<BlogPost>? Summaries;

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
            Summaries = JsonConvert.DeserializeObject<List<BlogPost>>(result);
        }
    }

    public void Add(BlogPost blogPost)
    {
        if (blogPost == null)
        {
            throw new ArgumentNullException(nameof(blogPost));
        }

        if (Summaries is null)
        {
            Summaries = new List<BlogPost>();
        }

        if (blogPost.BlogPostMarkdown?.Length > 500)
        {
            blogPost.BlogPostMarkdown = blogPost.BlogPostMarkdown.Substring(0, 500);
        }

        Summaries.Add(blogPost);
    }

    public void Update(BlogPost blogPost)
    {
        if (Summaries == null || !Summaries.Any(bp => bp.Id == blogPost.Id))
        {
            return;
        }

        var summary = Summaries.Find(summary => summary.Id == blogPost.Id);
        if (summary is not null)
        {
            summary.Title = blogPost.Title;
            summary.Tags = blogPost.Tags;
            summary.Author = blogPost.BlogPostMarkdown![..500];
        }
    }

    public void Remove(Guid id)
    {
        if (Summaries == null || !Summaries.Any(bp => bp.Id == id))
        {
            return;
        }

        var summary = Summaries.First(s => s.Id == id);
        Summaries.Remove(summary);
    }
}
