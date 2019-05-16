
using ObjectEditor.Json;

namespace Emigre.Data
{
    [Category("Subway")]
    public class PhoneEvent : StoryEvent
    {
        public bool show = true;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            show = fields.add(show, "show");
        }

        public override string ToString()
        {
            return (show ? "Show" : "Hide") + " Phone";
        }
    }
}
