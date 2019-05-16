using ObjectEditor.Json;

namespace HearthEditor.Data
{
    public class Resource : GameData
    {
        public string name;
        [FieldTag(FieldTags.Image, "icon")]
        public string icon;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            icon = fields.add(icon, "icon");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
