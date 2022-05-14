using Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Client.Services;

public class BlogPostService
{
    private readonly HttpClient http;
    private readonly NavigationManager navigationManager;
    private List<BlogPost> blogPosts = new();

    public BlogPostService(
        HttpClient http,
        NavigationManager navigationManager)
    {
        this.http = http ?? throw new ArgumentNullException(nameof(http));
        this.navigationManager = navigationManager ?? throw new ArgumentNullException (nameof(navigationManager));
    }

    public async Task<BlogPost?> GetBlogPost(Guid blogPostId)
    {
        var result = await http.GetAsync($"api/blogposts/{blogPostId}");
        if (!result.IsSuccessStatusCode)
        {
            navigationManager.NavigateTo("404");
            return null;
        }
        var serializedPost = await result.Content.ReadAsStringAsync();
        BlogPost? blogPost = JsonConvert.DeserializeObject<BlogPost>(serializedPost);

        if (blogPost is null)
        {
            navigationManager.NavigateTo("404");
            return null;
        }

        blogPosts.Add(blogPost);

        return blogPosts.FirstOrDefault(bp => bp.Id == blogPostId);
    }

}
