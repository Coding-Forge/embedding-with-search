using System;
using Azure;
using Azure.AI.OpenAI;
using System.ClientModel;

namespace vectorsearch {

    class Program
    {
        /// <summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your query:");
            string userQuery = Console.ReadLine();

            string endpoint = "https://demomeai.openai.azure.com";
            string apiKey = "";
            string deploymentId = "text-embedding-ada-002";

            Uri oaiEndpoint = new (endpoint);
            string oaiKey = apiKey;

            AzureKeyCredential credentials = new (oaiKey);

            OpenAIClient openAIClient = new (oaiEndpoint, credentials);

            EmbeddingsOptions embeddingOptions = new()
            {
                DeploymentName = deploymentId,
                Input = { userQuery },
            };

            var returnValue = openAIClient.GetEmbeddings(embeddingOptions);

            foreach (float item in returnValue.Value.Data[0].Embedding.ToArray())
            {
                Console.WriteLine(item);
            }

            string searchServiceName = "your-search-service-name";
            string indexName = "your-index-name";
            string apiKeySearch = "your-search-api-key";

            SearchClient searchClient = new SearchClient(
                new Uri($"https://{searchServiceName}.search.windows.net"),
                indexName,
                new AzureKeyCredential(apiKeySearch)
            );

            var vector = returnValue.Value.Data[0].Embedding.ToArray();

            var searchOptions = new SearchOptions
            {
                Vector = new SearchVector
                {
                    Value = vector,
                    K = 5 // Number of nearest neighbors to return
                }
            };

            SearchResults<SearchDocument> searchResults = searchClient.Search<SearchDocument>("", searchOptions);

            foreach (SearchResult<SearchDocument> result in searchResults.GetResults())
            {
                Console.WriteLine($"Document found: {result.Document["id"]}");
            }
        }
    }
}