using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Transition")]
    public class StartJourneyEvent : StoryEvent
    {

        public readonly Reference<Journey> journey = new Reference<Journey>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(journey, "journey");
        }

        public override string ToString()
        {
            return "Start " + journey;
        }
    }
}
