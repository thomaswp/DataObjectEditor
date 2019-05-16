
using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Subway")]
    public class ShowBookEvent : StoryEvent
    {
        public float fadeIn, fadeOut;
        public bool show = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fadeIn = fields.add(fadeIn, "fadeIn");
            fadeOut = fields.add(fadeOut, "fadeOut");
        }

        public override string ToString()
        {
            return "Show Book";
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
