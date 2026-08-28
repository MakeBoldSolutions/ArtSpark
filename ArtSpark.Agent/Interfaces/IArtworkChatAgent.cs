using ArtSpark.Agent.Models;
using ArtSpark.Agent.Personas;
namespace ArtSpark.Agent.Interfaces
{
    public interface IArtworkChatAgent
    {
        Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
        Task<List<string>> GenerateConversationStartersAsync(ArtworkData artwork, ChatPersona persona, CancellationToken cancellationToken = default);
        Task<List<string>> GenerateSuggestedQuestionsAsync(ArtworkData artwork, string lastMessage, ChatPersona persona, CancellationToken cancellationToken = default);
        Task<string> AnalyzeArtworkVisuallyAsync(ArtworkData artwork, string? specificQuestion = null, CancellationToken cancellationToken = default);
    }
}
