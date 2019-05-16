namespace Emigre.Data
{

    using ObjectEditor.Json;

    [Category("UI")]
    public class OverlayEvent : AbstractOverlayEvent
    {

        public string title;
        public string subtitle;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            title = fields.add(title, "title");
            subtitle = fields.add(subtitle, "subtitle");
        }

        public override string ToString()
        {
            return title + " / " + subtitle;
        }
    }

}