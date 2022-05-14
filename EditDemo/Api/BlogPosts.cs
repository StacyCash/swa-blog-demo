using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

using Models;
using System;

namespace CosmosDBTest;

public static class BlogPosts
{
    public static object UriFactory { get; private set; }

    [FunctionName($"{nameof(BlogPosts)}_Get")]
    public static IActionResult GetAllBlogPosts(
        [HttpTrigger(AuthorizationLevel.Anonymous,
            "get", Route = "blogposts")] HttpRequest req,
        [CosmosDB("SwaBlog", "BlogContainer",
            Connection = "CosmosDbConnectionString",
            SqlQuery = @"
                SELECT
                c.id,
                c.Title,
                c.Author,
                c.PublishedDate,
                LEFT(c.BlogPostMarkdown, 500)
                		As BlogPostMarkdown,
                Length(c.BlogPostMarkdown) <= 500
                		As PreviewIsComplete,
                c.Tags
                FROM c
                WHERE c.Status = 2")
            ] IEnumerable<BlogPost> blogPosts,
        ILogger log)
    {
        return new OkObjectResult(blogPosts);
    }

    [FunctionName($"{nameof(BlogPosts)}_GetId")]
    public static IActionResult GetBlogPost(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "blogposts/{id}")]
                HttpRequest req,
        [CosmosDB("SwaBlog", "BlogContainer",
                Connection = "CosmosDbConnectionString",
                SqlQuery = @"SELECT
                    c.id,
                    c.Title,
                    c.Author,
                    c.PublishedDate,
                    c.BlogPostMarkdown,
                    c.Status,
                    c.Tags
                    FROM c
                    WHERE c.id = {id}")
            ] IEnumerable<BlogPost> blogposts,
        ILogger log)
    {
        if (blogposts.ToArray().Length == 0)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(blogposts.First());
    }

    [FunctionName($"{nameof(BlogPosts)}_Post")]
    public static IActionResult PostBlogPost(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "blogposts")]
            BlogPost blogPost,
        [CosmosDB("SwaBlog", "BlogContainer",
            Connection = "CosmosDbConnectionString")]out dynamic document,
        ILogger log)
    {
        if (blogPost.Id != null)
        {
            throw new ArgumentException($"Blog post already has ID: {blogPost.Id}");
        }

        blogPost.Id = Guid.NewGuid();

        document = new
        {
            id = blogPost.Id.ToString(),
            Title = blogPost.Title,
            Author = blogPost.Author,
            PublishedDate = blogPost.PublishedDate,
            Tags = blogPost.Tags,
            BlogPostMarkdown = blogPost.BlogPostMarkdown,
            Status = 2
        };

        return new OkObjectResult(blogPost);
    }

    [FunctionName($"{nameof(BlogPosts)}_Put")]
    public static IActionResult PutBlogPost(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put",
            Route = "blogposts")]
            BlogPost blogPost,
        [CosmosDB("SwaBlog", "BlogContainer",
            Connection = "CosmosDbConnectionString")]out dynamic document,
        ILogger log)
    {
        document = new
        {
            id = blogPost.Id.ToString(),
            Title = blogPost.Title,
            Author = blogPost.Author,
            PublishedDate = blogPost.PublishedDate,
            Tags = blogPost.Tags,
            BlogPostMarkdown = blogPost.BlogPostMarkdown,
            Status = 2
        };

        return new OkObjectResult(blogPost);
    }
}
