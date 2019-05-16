using Emigre.Json;

namespace Emigre.Data
{
    [Category("Transition", "Control", "UI")]
    public class DiscoverLocationEvent : AbstractOverlayEvent
    {
        public readonly Reference<Location> location = new Reference<Location>();
        public bool discovered = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(location, "location");
            discovered = fields.add(discovered, "discovered");
        }

        public override string ToString()
        {
            return (discovered ? "Discovered " : "Lost ") + location;
        }
    }
}
