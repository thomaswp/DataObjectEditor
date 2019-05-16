using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Sprite")]
    public class ScreenEvent : StoryEvent
    {
        public float alpha;
        public float duration;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            alpha = fields.add(alpha, "alpha");
            duration = fields.add(duration, "duration");
        }

        public override string ToString()
        {
            return "Fade to " + alpha + " over " + duration + "s";
        }

        public override Action GetTransitionAction()
        {
            return Action.NextEvent;
        }
    }
}
