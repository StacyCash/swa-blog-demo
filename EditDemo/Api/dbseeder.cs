//using Microsoft.Azure.Cosmos;
//using Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace CosmosDBPlayground.DatabaseSeeding;

///// <summary>
///// Creates a database and container inside of an Azure CosmosDB account
/////   for the Beginning Static Web Apps with Blazor book and seeds with 7 blog posts
///// </summary>
//internal class DatabaseSeeder : IDisposable
//{
//    const string EndpointUri = "<Your CosmosDB Endpoint URL>";
//    const string PrimaryKey = "<Your CosmosDB Primary Key>";

//    const string databaseId = "SwaBlog";
//    const string containerId = "BlogContainer";

//    private readonly CosmosClient cosmosClient;

//    internal DatabaseSeeder()
//    {
//        cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
//    }

//    internal async Task CreateAndSeedDatabase()
//    {
//        var container = await CreateDatabaseAndContainer();
//        await SeedDatabase(container);
//    }

//    private async Task SeedDatabase(Container container)
//    {
//        List<BlogPost> allPosts = new()
//        {
//            APIAuthorization.BlogPost,
//            FirstStaticWebApp.BlogPost,
//            LoadingFromApi.BlogPost,
//            Navigation.BlogPost,
//            SettingUpTheAngularStaticWebAppPipeline.BlogPost,
//            YourFirstAngularApp.BlogPost,
//            YourFirstPage.BlogPost
//        };

//        foreach (var blogPost in allPosts)
//        {
//            var result = await container.CreateItemAsync(blogPost);
//            Console.WriteLine($"Cost of creating {blogPost.Title}: {result.RequestCharge}");
//        };

//        Console.WriteLine("Blog posts seeded");

//    }

//    private async Task<Container> CreateDatabaseAndContainer()
//    {
//        try
//        {
//            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
//            Console.WriteLine($"Created Database: {database.Id}");

//            Container container = await database.CreateContainerIfNotExistsAsync(containerId, "/Author");

//            Console.WriteLine($"Created Container: {container.Id}");

//            return container;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"{ex.Message}");
//            throw;
//        }


//    }

//    public void Dispose()
//    {
//        cosmosClient.Dispose();
//    }
//}
