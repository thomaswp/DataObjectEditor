using Emigre.Json;

namespace Emigre.Data
{
    [Category("Transition")]
    public class ChangeLocationEvent : StoryEvent, IReadOnlyScriptable
    {
        public readonly Reference<Location> location = new Reference<Location>();
        public bool incrementTime = true;
        public bool slowFade;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(location, "location");
            incrementTime = fields.add(incrementTime, "incrementTime");
            slowFade = fields.add(slowFade, "slowFade");
        }

        public override string ToString()
        {
            return "Move to " + location;
        }
    }
}
