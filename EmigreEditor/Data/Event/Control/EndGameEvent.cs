using Emigre.Json;

namespace Emigre.Data
{
    [Category("Control")]
    public class EndGameEvent : StoryEvent
    {
        public override string ToString()
        {
            return "End the game";
        }
    }
}
