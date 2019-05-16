using Emigre.Json;

namespace Emigre.Data
{
    [Category("Control")]
    public class EnablePageEvent : StoryEvent
    {
        public readonly Reference<Page> page = new Reference<Page>();
        public bool enabled;
        public bool reloadPages;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            fields.addReference(page, "page");
            enabled = fields.add(enabled, "enabled");
            reloadPages = fields.add(reloadPages, "reloadPages");
        }

        public override string ToString()
        {
            return (enabled ? "Enable " : "Disable ") + page.ToString();
        }

    }
}
