using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using PlanningAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlanningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatClient _chatClient;
        private readonly IConfiguration _configuration;

        public ChatController(
            ILogger<ChatController> logger,
            IChatClient chatClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _chatClient = chatClient;
            _configuration = configuration;
        }

        [HttpPost(Name = "Chat")]
        public async Task<string> Chat([FromBody] string message)
        {
            try
            {
                // 1. Get MCP Endpoint URI
                var endpointUri = _configuration["AI:MCPServiceUri"]
                    ?? throw new InvalidOperationException("MCPServiceUri is not configured in appsettings.");

                var endpoint = new Uri(endpointUri);

                // 2. Initialize MCP Transport and Client safely
                var httpTransport = new HttpClientTransport(new HttpClientTransportOptions
                {
                    Endpoint = endpoint
                });

                await using var mcpClient = await McpClient.CreateAsync(httpTransport);

                // 3. Fetch tools from the MCP server
                var mcpTools = await mcpClient.ListToolsAsync();

                // 4. Manually bridge MCP tools into AIFunction definitions.
                // This converts dynamic MCP tool signatures into standard AIFunctions,
                // bypassing GeminiDotnet's internal TextContent/Object source generation exception.
                var aiFunctions = mcpTools.Select(tool => AIFunctionFactory.Create(
                    async (JsonElement args) =>
                    {
                        var dictionaryArgs = JsonSerializer.Deserialize<Dictionary<string, object>>(args.GetRawText());
                        var result = await mcpClient.CallToolAsync(tool.Name, dictionaryArgs ?? new());
                        return result;
                    },
                    tool.Name,
                    tool.Description ?? string.Empty
                )).Cast<AITool>().ToList();

                // 5. Setup Chat History
                var messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, AiPrompt.SystemPrompt),
                    new ChatMessage(ChatRole.User, message)
                };

                // 6. Request complete non-streaming response with wrapped functions
                var response = await _chatClient.GetResponseAsync(
                    messages,
                    new ChatOptions
                    {
                        Tools = aiFunctions
                    }
                );

                return response.Text ?? "No response generated.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing full chat request with MCP tools.");
                return $"Error: {ex.Message}";
            }
        }
    }
}