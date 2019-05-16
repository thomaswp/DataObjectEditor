
using Emigre.Json;

namespace Emigre.Data
{
    [Category("Subway")]
    public class ShowTitleEvent : StoryEvent
    {
        public float fadeIn, fadeOut, pause;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fadeIn = fields.add(fadeIn, "fadeIn");
            fadeOut = fields.add(fadeOut, "fadeOut");
            pause = fields.add(pause, "pause");
        }

        public override string ToString()
        {
            return "Show Title";
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
