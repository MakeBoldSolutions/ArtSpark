using ArtSpark.Agent.Models;
using ArtSpark.Agent.Personas;
namespace ArtSpark.Agent.Interfaces
{
    public interface IPersonaHandler
    {
        string GenerateSystemPrompt(ArtworkData artwork);
        Task<string> ProcessMessageAsync(string message, ArtworkData artwork, List<ChatMessage> history);
        Task<List<string>> GenerateConversationStartersAsync(ArtworkData artwork);
        ChatPersona PersonaType { get; }
    }
}
