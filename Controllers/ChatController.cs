//using FinAxisLeaseBudgeting.Data;
//using FinAxisLeaseBudgeting.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.AI;
//using ModelContextProtocol.Client;
//using PlanningAPI.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Text.Json;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace PlanningAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ChatController : ControllerBase
//    {
//        private readonly ILogger<ChatController> _logger;
//        private readonly IChatClient _chatClient;
//        private readonly IConfiguration _configuration;
//        private readonly FinAxisDbContext _context;

//        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(4);

//        public ChatController(
//            ILogger<ChatController> logger,
//            IChatClient chatClient,
//            IConfiguration configuration,
//            FinAxisDbContext context)
//        {
//            _logger = logger;
//            _chatClient = chatClient;
//            _configuration = configuration;
//            _context = context;
//        }

//        [HttpGet("sessions")]
//        public async Task<IActionResult> GetUserChatSessions([FromQuery] int userId)
//        {
//            if (userId <= 0)
//            {
//                return BadRequest(new { success = false, error = "A valid UserId is required to fetch sessions." });
//            }

//            try
//            {
//                var sessions = await _context.ChatHistories
//                    .Where(h => h.UserId == userId)
//                    .GroupBy(h => h.SessionId)
//                    .Select(g => new
//                    {
//                        SessionId = g.Key,
//                        Title = g.OrderBy(h => h.CreatedAt).Select(h => h.UserQuery).FirstOrDefault(),
//                        LastUpdated = g.Max(h => h.CreatedAt)
//                    })
//                    .OrderByDescending(s => s.LastUpdated)
//                    .ToListAsync();

//                return Ok(sessions);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving sessions for user: {UserId}", userId);
//                return StatusCode(500, new { success = false, error = "Failed to retrieve user sessions safely." });
//            }
//        }

//        [HttpGet("history")]
//        public async Task<IActionResult> GetChatHistory([FromQuery] string sessionId, [FromQuery] int userId)
//        {
//            if (string.IsNullOrWhiteSpace(sessionId) || userId <= 0)
//            {
//                return BadRequest(new { success = false, error = "A valid SessionId and UserId are required." });
//            }

//            try
//            {
//                var history = await _context.ChatHistories
//                    .Where(h => h.SessionId == sessionId && h.UserId == userId)
//                    .OrderBy(h => h.CreatedAt)
//                    .Select(h => new
//                    {
//                        h.Id,
//                        h.UserQuery,
//                        h.AssistantResponse,
//                        h.CreatedAt
//                    })
//                    .ToListAsync();

//                if (!history.Any())
//                {
//                    return NotFound(new { success = false, error = "Session not found or unauthorized." });
//                }

//                return Ok(history);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving history for session {SessionId} and user {UserId}", sessionId, userId);
//                return StatusCode(500, new { success = false, error = "Failed to retrieve history safely." });
//            }
//        }

//        [HttpPost(Name = "Chat")]
//        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Prompt) || request.UserId <= 0)
//            {
//                return BadRequest(new { success = false, response = "Prompt and a valid UserId cannot be empty." });
//            }

//            string sessionId = string.IsNullOrWhiteSpace(request.SessionId)
//                ? Guid.NewGuid().ToString()
//                : request.SessionId;

//            // 1. Normalize query template to allow cross-session/cross-user data matching (e.g. ignoring numeric thresholds)
//            string normalizedQueryTemplate = NormalizeQueryTemplate(request.Prompt);
//            string queryHash = ComputeSha256Hash(normalizedQueryTemplate);

//            try
//            {
//                var cacheExpiryThreshold = DateTime.UtcNow.Subtract(CacheTtl);

//                // 2. GLOBAL CACHE LOOKUP: Check if a similar query dataset was fetched recently 
//                // across ANY session or user, allowing us to reuse its data payload.
//                var cachedRecord = await _context.ChatHistories
//                    .Where(h => h.QueryHash == queryHash && h.CreatedAt >= cacheExpiryThreshold)
//                    .OrderByDescending(h => h.CreatedAt)
//                    .FirstOrDefaultAsync();

//                string finalAiResponse;
//                string source;

//                if (cachedRecord != null)
//                {
//                    _logger.LogInformation("Global Cross-Session Cache HIT for query pattern hash: {Hash}", queryHash);

//                    // 3. REUSE CACHED DATA: Skip heavy MCP tools/database calls entirely and inject the prior dataset response
//                    finalAiResponse = await ExecuteCachedDataPipelineAsync(request.Prompt, cachedRecord.AssistantResponse);
//                    source = "global-cache";
//                }
//                else
//                {
//                    _logger.LogInformation("Global Cross-Session Cache MISS. Executing live MCP/LLM pipeline.");
//                    finalAiResponse = await ExecuteLiveMcpPipelineAsync(sessionId, request.Prompt);
//                    source = "live";

//                    var newHistoryEntry = new ChatHistoryMessage
//                    {
//                        UserId = request.UserId,
//                        SessionId = sessionId,
//                        UserQuery = request.Prompt,
//                        QueryHash = queryHash,
//                        AssistantResponse = finalAiResponse,
//                        CreatedAt = DateTime.UtcNow
//                    };

//                    _context.ChatHistories.Add(newHistoryEntry);
//                    await _context.SaveChangesAsync();
//                }

//                // 4. Record this turn into the current user's session history
//                //var newHistoryEntry = new ChatHistoryMessage
//                //{
//                //    UserId = request.UserId,
//                //    SessionId = sessionId,
//                //    UserQuery = request.Prompt,
//                //    QueryHash = queryHash,
//                //    AssistantResponse = finalAiResponse,
//                //    CreatedAt = DateTime.UtcNow
//                //};

//                //_context.ChatHistories.Add(newHistoryEntry);
//                //await _context.SaveChangesAsync();

//                return Ok(new
//                {
//                    success = true,
//                    sessionId = sessionId,
//                    response = finalAiResponse,
//                    source = source
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing secure chat message.");
//                return StatusCode(500, new { success = false, response = $"System Error: {ex.Message}" });
//            }
//        }

//        private async Task<string> ExecuteLiveMcpPipelineAsync(string sessionId, string userMessage)
//        {
//            var endpointUri = _configuration["AI:MCPServiceUri"]
//                ?? throw new InvalidOperationException("MCPServiceUri is not configured.");

//            var endpoint = new Uri(endpointUri);
//            var httpTransport = new HttpClientTransport(new HttpClientTransportOptions { Endpoint = endpoint });

//            await using var mcpClient = await McpClient.CreateAsync(httpTransport);
//            var mcpTools = await mcpClient.ListToolsAsync();

//            var aiFunctions = mcpTools.Select(tool => AIFunctionFactory.Create(
//                async (JsonElement args) =>
//                {
//                    var dictionaryArgs = JsonSerializer.Deserialize<Dictionary<string, object>>(args.GetRawText());
//                    var result = await mcpClient.CallToolAsync(tool.Name, dictionaryArgs ?? new());
//                    return result;
//                },
//                tool.Name,
//                tool.Description ?? string.Empty
//            )).Cast<AITool>().ToList();

//            var messages = new List<ChatMessage>
//            {
//                new ChatMessage(ChatRole.System, AiPrompt.SystemPrompt)
//            };

//            // Include local session history if available for multi-turn thread memory
//            if (!string.IsNullOrWhiteSpace(sessionId))
//            {
//                var pastHistory = await _context.ChatHistories
//                    .Where(h => h.SessionId == sessionId)
//                    .OrderBy(h => h.CreatedAt)
//                    .ToListAsync();

//                foreach (var past in pastHistory)
//                {
//                    if (!string.IsNullOrWhiteSpace(past.UserQuery))
//                        messages.Add(new ChatMessage(ChatRole.User, past.UserQuery));

//                    if (!string.IsNullOrWhiteSpace(past.AssistantResponse))
//                        messages.Add(new ChatMessage(ChatRole.Assistant, past.AssistantResponse));
//                }
//            }

//            messages.Add(new ChatMessage(ChatRole.User, userMessage));

//            var response = await _chatClient.GetResponseAsync(messages, new ChatOptions { Tools = aiFunctions });
//            return response.Text ?? "No response generated.";
//        }

//        private async Task<string> ExecuteCachedDataPipelineAsync(string userPrompt, string previousCachedDatasetResponse)
//        {
//            var messages = new List<ChatMessage>
//            {
//                new ChatMessage(ChatRole.System, AiPrompt.SystemPrompt +
//                    "\n\nDATA REUSE INSTRUCTION: The user is asking a follow-up or filtering question. " +
//                    "Below is the raw dataset previously retrieved from the database in a cached record. " +
//                    "DO NOT call any MCP tools or query the database. Process, filter, or apply assumptions " +
//                    "directly using this provided dataset to conserve tokens and execution time."),

//                new ChatMessage(ChatRole.Assistant, $"Cached Dataset Result from Prior Session:\n{previousCachedDatasetResponse}"),

//                new ChatMessage(ChatRole.User, userPrompt)
//            };

//            // Executes directly against the LLM without spinning up McpClient or running tool discovery
//            var response = await _chatClient.GetResponseAsync(messages);
//            return response.Text ?? "No response generated from cached data.";
//        }

//        private static string NormalizeQueryTemplate(string rawPrompt)
//        {
//            string normalized = rawPrompt.Trim().ToLowerInvariant();
//            // Replaces numbers with placeholders so queries with different parameter limits 
//            // (e.g., rent < 2000 vs rent < 1000) match the same global data cache entry template.
//            normalized = Regex.Replace(normalized, @"\b\d+\b", "{value}");
//            return normalized;
//        }

//        private static string ComputeSha256Hash(string rawData)
//        {
//            using var sha256Hash = SHA256.Create();
//            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
//            var builder = new StringBuilder();
//            foreach (var b in bytes) { builder.Append(b.ToString("x2")); }
//            return builder.ToString();
//        }
//    }

//    public class ChatRequest
//    {
//        public int UserId { get; set; }
//        public string SessionId { get; set; } = string.Empty;
//        public string Prompt { get; set; } = string.Empty;
//    }
//}
using FinAxisLeaseBudgeting.Data;
using FinAxisLeaseBudgeting.Interfaces;
using FinAxisLeaseBudgeting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using PlanningAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        private readonly FinAxisDbContext _context;
        private readonly IUserPropertySecurityRepository _securityRepository;

        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(4);

        public ChatController(
            ILogger<ChatController> logger,
            IChatClient chatClient,
            IConfiguration configuration,
            FinAxisDbContext context,
            IUserPropertySecurityRepository securityRepository)
        {
            _logger = logger;
            _chatClient = chatClient;
            _configuration = configuration;
            _context = context;
            _securityRepository = securityRepository;
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetUserChatSessions([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { success = false, error = "A valid UserId is required to fetch sessions." });
            }

            try
            {
                var sessions = await _context.ChatHistories
                    .Where(h => h.UserId == userId)
                    .GroupBy(h => h.SessionId)
                    .Select(g => new
                    {
                        SessionId = g.Key,
                        Title = g.OrderBy(h => h.CreatedAt).Select(h => h.UserQuery).FirstOrDefault(),
                        LastUpdated = g.Max(h => h.CreatedAt)
                    })
                    .OrderByDescending(s => s.LastUpdated)
                    .ToListAsync();

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sessions for user: {UserId}", userId);
                return StatusCode(500, new { success = false, error = "Unable to get the data due to some internal issue. Please try after sometime." });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] string sessionId, [FromQuery] int userId)
        {
            if (string.IsNullOrWhiteSpace(sessionId) || userId <= 0)
            {
                return BadRequest(new { success = false, error = "A valid SessionId and UserId are required." });
            }

            try
            {
                var history = await _context.ChatHistories
                    .Where(h => h.SessionId == sessionId && h.UserId == userId)
                    .OrderBy(h => h.CreatedAt)
                    .Select(h => new
                    {
                        h.Id,
                        h.UserQuery,
                        h.AssistantResponse,
                        h.CreatedAt
                    })
                    .ToListAsync();

                if (!history.Any())
                {
                    return NotFound(new { success = false, error = "Session not found or unauthorized." });
                }

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for session {SessionId} and user {UserId}", sessionId, userId);
                return StatusCode(500, new { success = false, error = "Unable to get the data due to some internal issue. Please try after sometime." });
            }
        }

        [HttpPost(Name = "Chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt) || request.UserId <= 0)
            {
                return BadRequest(new { success = false, response = "Prompt and a valid UserId cannot be empty." });
            }

            string sessionId = string.IsNullOrWhiteSpace(request.SessionId)
                ? Guid.NewGuid().ToString()
                : request.SessionId;

            string normalizedQueryTemplate = NormalizeQueryTemplate(request.Prompt);
            string queryHash = ComputeSha256Hash(normalizedQueryTemplate);

            try
            {
                // Retrieve user's mapped access boundaries from repository
                string allowedScopeSummary = await _securityRepository.GetUserAllowedScopeSummaryAsync(request.UserId);

                var cacheExpiryThreshold = DateTime.UtcNow.Subtract(CacheTtl);

                var cachedRecord = await _context.ChatHistories
                    .Where(h => h.QueryHash == queryHash && h.CreatedAt >= cacheExpiryThreshold)
                    .OrderByDescending(h => h.CreatedAt)
                    .FirstOrDefaultAsync();

                string finalAiResponse;
                string source;

                if (cachedRecord != null)
                {
                    _logger.LogInformation("Global Cross-Session Cache HIT for query pattern hash: {Hash}", queryHash);
                    finalAiResponse = await ExecuteCachedDataPipelineAsync(request.Prompt, cachedRecord.AssistantResponse, allowedScopeSummary);
                    source = "global-cache";
                }
                else
                {
                    _logger.LogInformation("Global Cross-Session Cache MISS. Executing live MCP/LLM pipeline.");
                    finalAiResponse = await ExecuteLiveMcpPipelineAsync(sessionId, request.Prompt, request.UserId, allowedScopeSummary);
                    source = "live";

                    var newHistoryEntry = new ChatHistoryMessage
                    {
                        UserId = request.UserId,
                        SessionId = sessionId,
                        UserQuery = request.Prompt,
                        QueryHash = queryHash,
                        AssistantResponse = finalAiResponse,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.ChatHistories.Add(newHistoryEntry);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    success = true,
                    sessionId = sessionId,
                    response = finalAiResponse,
                    source = source
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing secure chat message.");
                return StatusCode(500, new { success = false, response = "Unable to get the data due to some internal issue. Please try after sometime." });
            }
        }

        private async Task<string> ExecuteLiveMcpPipelineAsync(string sessionId, string userMessage, long userId, string allowedScopeSummary)
        {
            var endpointUri = _configuration["AI:MCPServiceUri"]
                ?? throw new InvalidOperationException("MCPServiceUri is not configured.");

            var endpoint = new Uri(endpointUri);
            var httpTransport = new HttpClientTransport(new HttpClientTransportOptions { Endpoint = endpoint });

            await using var mcpClient = await McpClient.CreateAsync(httpTransport);
            var mcpTools = await mcpClient.ListToolsAsync();

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

            // Inject security authorization rules into system prompt
            string securityEnforcedSystemPrompt = $"{AiPrompt.SystemPrompt}\n\n" +
                $"========================================\n" +
                $"SECURITY ACCESS SCOPE FOR USER ID {userId}:\n" +
                $"{allowedScopeSummary}\n" +
                $"MAPPING RULES:\n" +
                $"- If an entity has null or empty property/unit mappings, all properties and units under that entity are allowed.\n" +
                $"- If specific properties/units are mapped, restrict responses and tool outputs strictly to those items.\n" +
                $"- Exclude and filter out any unauthorized entity, property, or unit data completely from the response.\n" +
                $"=========================================";

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, securityEnforcedSystemPrompt)
            };

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                var pastHistory = await _context.ChatHistories
                    .Where(h => h.SessionId == sessionId)
                    .OrderBy(h => h.CreatedAt)
                    .ToListAsync();

                foreach (var past in pastHistory)
                {
                    if (!string.IsNullOrWhiteSpace(past.UserQuery))
                        messages.Add(new ChatMessage(ChatRole.User, past.UserQuery));

                    if (!string.IsNullOrWhiteSpace(past.AssistantResponse))
                        messages.Add(new ChatMessage(ChatRole.Assistant, past.AssistantResponse));
                }
            }

            messages.Add(new ChatMessage(ChatRole.User, userMessage));

            var response = await _chatClient.GetResponseAsync(messages, new ChatOptions { Tools = aiFunctions });
            return response.Text ?? "No response generated.";
        }

        private async Task<string> ExecuteCachedDataPipelineAsync(string userPrompt, string previousCachedDatasetResponse, string allowedScopeSummary)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, AiPrompt.SystemPrompt +
                    "\n\nDATA REUSE INSTRUCTION: The user is asking a follow-up or filtering question. " +
                    "Below is the raw dataset previously retrieved from the database in a cached record. " +
                    "DO NOT call any MCP tools or query the database. Process, filter, or apply assumptions " +
                    "directly using this provided dataset to conserve tokens and execution time.\n\n" +
                    $"SECURITY ACCESS SCOPE RESTRICTION:\n{allowedScopeSummary}"),

                new ChatMessage(ChatRole.Assistant, $"Cached Dataset Result from Prior Session:\n{previousCachedDatasetResponse}"),

                new ChatMessage(ChatRole.User, userPrompt)
            };

            var response = await _chatClient.GetResponseAsync(messages);
            return response.Text ?? "No response generated from cached data.";
        }

        private static string NormalizeQueryTemplate(string rawPrompt)
        {
            string normalized = rawPrompt.Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"\b\d+\b", "{value}");
            return normalized;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes) { builder.Append(b.ToString("x2")); }
            return builder.ToString();
        }
    }

    public class ChatRequest
    {
        public int UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
    }
}