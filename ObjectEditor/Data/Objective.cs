using Emigre.Json;

namespace Emigre.Data
{
    public class Objective : GameData
    {
        public enum State
        {
            Undiscovered, Active, Completed, Failed
        }

        public string name;
        [FieldTag(FieldTags.Multiline)]
        public string description;

        public override void AddFields(Json.FieldData fields)
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
