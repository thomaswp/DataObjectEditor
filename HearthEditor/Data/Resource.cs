using ObjectEditor.Json;

namespace HearthEditor.Data
{
    public class Resource : GameData
    {
        public string name;
        [FieldTag(FieldTags.Image, "icon")]
        public string icon;

        public override string ToString()
        {
            return name;
        }
    }
}
