using ArtSpark.Agent.Models;
using ArtSpark.Agent.Personas;
namespace ArtSpark.Agent.Interfaces
{
    public interface IPersonaFactory
    {
        IPersonaHandler CreatePersona(ChatPersona persona, ArtworkData artwork);
    }
}
