using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using ModelContextProtocol.Client;

var mcpClient = await GetMCPClient();
var tools = await mcpClient.ListToolsAsync();
Console.WriteLine("Tools in Server:");

foreach (var tool in tools)
{
    Console.WriteLine($"{tool.Name}: {tool.Description}");
}
Console.WriteLine();

Kernel kernel = Kernel
    .CreateBuilder()
    .AddOllamaChatCompletion("granite3.3", new Uri("http://localhost:11434"))
    .Build();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
kernel.Plugins.AddFromFunctions(
    "EM_MCP",
    tools.Select(aiFunction => aiFunction.AsKernelFunction())
);

OllamaPromptExecutionSettings executionSettings = new()
{
    Temperature = 0,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
};

OpenApiFunctionExecutionParameters openApiParams = new() { EnablePayloadNamespacing = true };

//await kernel.ImportPluginFromOpenApiAsync("EMMcpOpenApiTools",
//    new Uri("https://localhost:7157/openapi/v1.json"), openApiParams);

while (true)
{
    Console.Write("Enter your prompt: ");
    string? prompt = Console.ReadLine();

    await foreach (
        var x in kernel.InvokePromptStreamingAsync<string>(prompt, new(executionSettings))
    )
    {
        Console.Write(x);
    }

    Console.WriteLine();
}

static async Task<McpClient> GetMCPClient()
{
    McpClientOptions options = new()
    {
        ClientInfo = new() { Name = "EM_MCP_Client", Version = "1.0.0" },
    };

    var clientTransport = new HttpClientTransport(
        new()
        {
            Name = "EM-MCP-Client",
            Endpoint = new("https://localhost:7077/"),
            TransportMode = HttpTransportMode.StreamableHttp,
            ConnectionTimeout = TimeSpan.FromMinutes(1),
            OAuth = new()
            {
                ClientId = "backend-1",
                ClientSecret = "UfIMrte6w4gRCt2PYGL6ywPDtr1xR9cB",
                Scopes = ["openid", "profile"],
                RedirectUri = new Uri("http://localhost:1234/signin-oidc"),
                AuthorizationRedirectDelegate = HandleAuthorizationUrlAsync,
            },
        }
    );

    return await McpClient.CreateAsync(clientTransport, options);
}

/// Handles the OAuth authorization URL by starting a local HTTP server and opening a browser.
/// This implementation demonstrates how SDK consumers can provide their own authorization flow.
/// </summary>
/// <param name="authorizationUrl">The authorization URL to open in the browser.</param>
/// <param name="redirectUri">The redirect URI where the authorization code will be sent.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The authorization code extracted from the callback, or null if the operation failed.</returns>
static async Task<string?> HandleAuthorizationUrlAsync(
    Uri authorizationUrl,
    Uri redirectUri,
    CancellationToken cancellationToken
)
{
    Console.WriteLine("Starting OAuth authorization flow...");
    Console.WriteLine($"Opening browser to: {authorizationUrl}");

    var listenerPrefix = redirectUri.GetLeftPart(UriPartial.Authority);
    if (!listenerPrefix.EndsWith('/'))
        listenerPrefix += "/";

    using var listener = new HttpListener();
    listener.Prefixes.Add(listenerPrefix);

    try
    {
        listener.Start();
        Console.WriteLine($"\nListening for OAuth callback on: {listenerPrefix}");

        OpenBrowser(authorizationUrl);

        var context = await listener.GetContextAsync();
        var query = HttpUtility.ParseQueryString(context.Request.Url?.Query ?? string.Empty);
        var code = query["code"];
        var error = query["error"];

        string responseHtml =
            "<html><body><h1>Authentication complete</h1><p>You can close this window now.</p></body></html>";
        byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.ContentType = "text/html";
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        context.Response.Close();

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Auth error: {error}");
            return null;
        }

        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("No authorization code received");
            return null;
        }

        Console.WriteLine("\nAuthorization code received successfully.\n");
        return code;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting auth code: {ex.Message}");
        return null;
    }
    finally
    {
        if (listener.IsListening)
            listener.Stop();
    }
}

/// <summary>
/// Opens the specified URL in the default browser.
/// </summary>
/// <param name="url">The URL to open.</param>
static void OpenBrowser(Uri url)
{
    try
    {
        var psi = new ProcessStartInfo { FileName = url.ToString(), UseShellExecute = true };
        Process.Start(psi);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error opening browser. {ex.Message}");
        Console.WriteLine($"Please manually open this URL: {url}");
    }
}
