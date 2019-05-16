
using Emigre.Json;

namespace Emigre.Data
{
    public enum Minigame
    {
        Language
    }

    [Category("Transition")]
    public class MinigameEvent : StoryEvent
    {
        public Minigame game;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            game = fields.addEnum(game, "game");
        }

        public override string ToString()
        {
            return "Play " + game;
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
