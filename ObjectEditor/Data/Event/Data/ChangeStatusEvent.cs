using Emigre.Json;

namespace Emigre.Data
{
    [Category("Data")]
    public class ChangeStatusEvent : StoryEvent
    {
        public readonly Reference<IHasStatus> item = new Reference<IHasStatus>();
        public HighlightStatus status;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(item, "location");
            status = fields.addEnum(status, "status");
        }

        public override string ToString()
        {
            return "Set " + item + " to " + status;
        }
    }
}
