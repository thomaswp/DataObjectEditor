
using Emigre.Json;

namespace Emigre.Data
{
    [Category("Data")]
    public class SetLodgingEvent : StoryEvent
    {
        public readonly Reference<Location> location = new Reference<Location>();

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(location, "location");
        }

    }
}
