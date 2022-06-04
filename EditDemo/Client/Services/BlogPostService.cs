using Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Text;

namespace Client.Services;

public class BlogPostService
{
    private readonly HttpClient http;
    private readonly NavigationManager navigationManager;
    private readonly BlogPostSummaryService blogPostSummaryService;
    private List<BlogPost> blogPosts = new();

    public BlogPostService(
        HttpClient http,
        NavigationManager navigationManager,
        BlogPostSummaryService blogPostSummaryService)
    {
        this.http = http ?? throw new ArgumentNullException(nameof(http));
        this.navigationManager = navigationManager ?? throw new ArgumentNullException (nameof(navigationManager));
        this.blogPostSummaryService = blogPostSummaryService ?? throw new ArgumentNullException(nameof(blogPostSummaryService));
    }

    public async Task<BlogPost?> GetBlogPost(Guid blogPostId)
    {
        if (blogPosts.Any(bp => bp.Id == blogPostId))
        {
            return blogPosts.FirstOrDefault(bp => bp.Id == blogPostId);
        }

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

        return blogPost;
    }

    public async Task<Guid> CreateBlogPost(BlogPost blogPost)
    {
        if (blogPost == null)
        {
            throw new ArgumentNullException(nameof(blogPost));
        }

        var content = JsonConvert.SerializeObject(blogPost);
        var data = new StringContent(content, Encoding.UTF8, "application/json");

        var result = await http.PostAsync("api/blogposts", data);
        var json = await result.Content.ReadAsStringAsync();
        BlogPost? savedBlogPost = JsonConvert.DeserializeObject<BlogPost>(json);
        blogPosts.Add(savedBlogPost!);
        blogPostSummaryService.Add(savedBlogPost!);

        return savedBlogPost!.Id!.Value;
    }

    public async Task UpdateBlogPost(BlogPost blogPost)
    {
        var content = JsonConvert.SerializeObject(blogPost);
        var data = new StringContent(content, Encoding.UTF8, "application/json");

        var result = await http.PutAsync("api/blogposts", data);
        var json = await result.Content.ReadAsStringAsync();
        BlogPost? savedBlogPost = JsonConvert.DeserializeObject<BlogPost>(json);

        int index = blogPosts.FindIndex(item => item.Id == savedBlogPost!.Id);
        if (index >= 0)
        {
            blogPosts[index] = savedBlogPost!;
        }

        blogPostSummaryService.Update(savedBlogPost!);
    }

    public void Delete(Guid id, string author)
    {
        http.DeleteAsync($"/api/blogposts/{id}/{author}");
        blogPosts.Remove(blogPosts.FirstOrDefault(bp => bp.Id == id));
        blogPostSummaryService.Remove(id);
    }

}
