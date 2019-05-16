namespace Emigre.Data
{
    public abstract class AbstractOverlayEvent : StoryEvent
    {
        public override StoryEvent.Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
