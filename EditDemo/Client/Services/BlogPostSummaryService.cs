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
            return;
        }

        if (!Summaries.Any(summary =>
            summary.Id == blogPost.Id && summary.Author == blogPost.Author))
        {
            var summary = new BlogPost
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                BlogPostMarkdown = blogPost.BlogPostMarkdown,
                PublishedDate = blogPost.PublishedDate,
                Tags = blogPost.Tags,
                Title = blogPost.Title
            };

            if (summary.BlogPostMarkdown?.Length > 500)
            {
                summary.BlogPostMarkdown = summary.BlogPostMarkdown[..500];
            }

            Summaries.Add(summary);
        }
        else
        {
            Replace(blogPost);
        }
    }

    public void Replace(BlogPost blogPost)
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
            summary.BlogPostMarkdown = blogPost.BlogPostMarkdown!;
            if (summary.BlogPostMarkdown.Length > 500)
            {
                summary.BlogPostMarkdown = summary.BlogPostMarkdown[..500];
            }
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
