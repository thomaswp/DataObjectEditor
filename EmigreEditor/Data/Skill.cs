
using ObjectEditor.Json;

namespace Emigre.Data
{
    public class Skill : GameData
    {
        public string name;

        [FieldTag(FieldTags.Multiline)]
        public string description;

        public int startValue;
        public int maxValue;

        public override void AddFields(FieldData fields)
        {
            base.AddFields(fields);
            name = fields.add(name, "name");
            description = fields.add(description, "description");
            startValue = fields.add(startValue, "startValue");
            maxValue = fields.add(maxValue, "maxValue");       
        }

        public override string ToString()
        {
            return name;
        }
    }
}
