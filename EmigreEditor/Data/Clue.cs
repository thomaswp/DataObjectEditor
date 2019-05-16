
using Emigre.Json;

namespace Emigre.Data
{
    public class Clue : GameData
    {
        public string name;
        [FieldTag(FieldTags.Multiline)]
        public string description;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            description = fields.add(description, "description");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
