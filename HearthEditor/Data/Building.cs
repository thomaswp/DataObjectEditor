using ObjectEditor.Json;

namespace HearthEditor.Data
{
    public class Building : GameData
    {
        public string name;
        [FieldTag(FieldTags.Image, "icon")]
        public string icon;
        [FieldTag(FieldTags.Image, "building")]
        public string sprite;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            icon = fields.add(icon, "icon");
            sprite = fields.add(sprite, "sprite");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
