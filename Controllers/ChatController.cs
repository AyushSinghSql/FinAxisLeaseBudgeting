using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using PlanningAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                // MCP Server Endpoint
                var endpointUri = _configuration["AI:MCPServiceUri"]
                    ?? throw new InvalidOperationException("MCPServiceUri is not configured in appsettings.");

                var endpoint = new Uri(endpointUri);

                // Initialize MCP Transport & Client
                var httpTransport = new HttpClientTransport(new HttpClientTransportOptions
                {
                    Endpoint = endpoint
                });

                await using var mcpClient = await McpClient.CreateAsync(httpTransport);

                // Fetch available tools from MCP server
                var mcpTools = await mcpClient.ListToolsAsync();

                // Setup Chat Messages
                var messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, AiPrompt.SystemPrompt),
                    new ChatMessage(ChatRole.User, message)
                };

                // Convert MCP tools into standard AIFunction instances to bypass GeminiDotnet's strict MEAI type mapper crash
                var aiFunctions = mcpTools.Select(t => AIFunctionFactory.Create(
                    async (object[] args) => await mcpClient.CallToolAsync(t.Name, args.ToDictionary(a => a?.ToString() ?? "", a => a)),
                    t.Name,
                    t.Description ?? string.Empty
                )).Cast<AIFunction>().ToList();

                // Get the complete full response using standard AIFunction definitions
                var response = await _chatClient.GetResponseAsync(
                    messages,
                    new ChatOptions
                    {
                        Tools = [.. aiFunctions]
                    }
                );

                return response.Text ?? "No response generated.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing full chat request.");
                return $"Error: {ex.Message}";
            }
        }
    }
}