using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Plugins.OpenApi;

using ModelContextProtocol.Client;

var mcpClient = await GetMCPClient();
var tools = await mcpClient.ListToolsAsync();

foreach (var tool in tools)
{
    Console.WriteLine($"{tool.Name}: {tool.Description}");
}

Kernel kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion("granite3.3", new Uri("http://localhost:11434"))
    .Build();

OllamaPromptExecutionSettings executionSettings = new()
{
    Temperature = 0,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};

OpenApiFunctionExecutionParameters openApiParams = new()
{
    EnablePayloadNamespacing = true,
};

//await kernel.ImportPluginFromOpenApiAsync("EMMcpOpenApiTools",
//    new Uri("https://localhost:7157/openapi/v1.json"), openApiParams);

while (true)
{
    Console.Write("Enter your prompt: ");
    string? prompt = Console.ReadLine();

    await foreach (var x in kernel.InvokePromptStreamingAsync<string>(prompt, new(executionSettings)))
    {
        Console.Write(x);
    }

    Console.WriteLine();
}



static async Task<IMcpClient> GetMCPClient()
{
    McpClientOptions options = new()
    {
        ClientInfo = new() { Name = "EM_MCP_Client", Version = "1.0.0" },
    };

    var clientTransport = new SseClientTransport(new SseClientTransportOptions
    {
        Name = "EM-MCP-Client",
        Endpoint = new("https://localhost:7077/"),
        TransportMode = HttpTransportMode.StreamableHttp,
        ConnectionTimeout = TimeSpan.FromMinutes(1),
        OAuth = new()
        {
            ClientName = "ProtectedEMMcpClient",
            ClientId = "backend-1",
            ClientSecret = "UfIMrte6w4gRCt2PYGL6ywPDtr1xR9cB",
            Scopes = ["openid", "profile"],
            RedirectUri = new Uri("http://localhost:7157/signin-oidc"),
            //AuthorizationRedirectDelegate = HandleAuthorizationUrlAsync,
        }
    });

    return await McpClientFactory.CreateAsync(clientTransport, options);
}

//static async Task<string?> HandleAuthorizationUrlAsync(Uri authorizationUri, Uri redirectUri, CancellationToken cancellationToken)
//{
//    Console.WriteLine($"Auth URL: {authorizationUri}");
//    Console.WriteLine($"Redirect URL: {redirectUri}");
//}