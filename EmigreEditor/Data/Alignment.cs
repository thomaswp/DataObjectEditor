using ObjectEditor.Json;

namespace Emigre.Data
{
    public class Alignment : GameData
    {
        public string leftAlign, rightAlign;

        [FieldTag(FieldTags.Multiline)]
        public string description;

        public override void AddFields(FieldData fields)
        {
            leftAlign = fields.add(leftAlign, "leftAlign");
            rightAlign = fields.add(rightAlign, "rightAlign");
            description = fields.add(description, "description");
            base.AddFields(fields);
        }

        public override string ToString()
        {
            return "[" + leftAlign + " <-> " + rightAlign + "]";
        }
    }
}
