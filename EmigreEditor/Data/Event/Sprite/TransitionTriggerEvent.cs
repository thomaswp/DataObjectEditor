
using Emigre.Json;

namespace Emigre.Data
{
    [Category("Sprite", "Transition")]
    public class TransitionTriggerEvent : StoryEvent
    {
        public string label;
        public float duration;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            label = fields.add(label, "label");
            duration = fields.add(duration, "duration");
        }

        public override string ToString()
        {
            return string.Format("Trigger {0} over {1}s", label, duration);
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
